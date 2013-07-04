using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Build.Tasks.Deployment.ManifestUtilities;
using System.IO;
using System.Net;

namespace PortableClickonce.Services
{
	public class DownloaderService
	{
		public event EventHandler<EventArgs<string>> DownloadStarted;
		public event EventHandler<EventArgs<KeyValuePair<int, object>>> DownloadProgress;
		public event EventHandler<EventArgs> DownloadFinished;
		public event EventHandler<EventArgs<Exception>> DownloadFailed;

		private System.ComponentModel.BackgroundWorker m_BgWorker;

		public DownloaderService()
		{
			m_BgWorker = new System.ComponentModel.BackgroundWorker();
			m_BgWorker.WorkerSupportsCancellation = true;
			m_BgWorker.WorkerReportsProgress = true;
			m_BgWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(m_BgWorker_DoWork);
			m_BgWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(m_BgWorker_ProgressChanged);
			m_BgWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(m_BgWorker_RunWorkerCompleted);
		}

		public Models.ClickonceSettings Settings { get; private set; }

		public void StartFullDownload(Models.ClickonceSettings settings)
		{
			if (m_BgWorker.IsBusy)
			{
				throw new Exception(PCResource.DownloadAlreadyInProgressMessage);
			}

			Settings = settings;
			settings.ClickonceUrl = settings.ClickonceUrl.Trim('/');
			if (DownloadStarted != null)
			{
				DownloadStarted(this, new EventArgs<string>("Go"));
			}

			m_BgWorker.RunWorkerAsync();
		}

		public void Cancel()
		{
			if (m_BgWorker != null
				&& m_BgWorker.IsBusy)
			{
				m_BgWorker.CancelAsync();
			}
		}

		public int IsLatestVersion(Models.ClickonceSettings settings)
		{
			if (settings == null)
			{
				return -1;
			}
			if (!settings.LastRunSuccess)
			{
				return -1;
			}
			if (settings.FullExeFileName == null)
			{
				return -1;
			}
			if (!System.IO.File.Exists(settings.FullExeFileName))
			{
				return -1;
			}
			DeployManifest deployManifest = null;
			try
			{
				string urlRedirect = null;
				deployManifest = DownloadDeployManifest(settings.ClickonceUrl, out urlRedirect);
				if (urlRedirect != settings.ClickonceUrl)
				{
					settings.ClickonceUrl = urlRedirect;
				}
			}
			catch(Exception ex)
			{
				return -2;
			}

			return deployManifest.AssemblyIdentity.Version == settings.Version ? 0 : -1;
		}

		#region BGWorker

		void m_BgWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
		{
			if (Settings == null)
			{
				throw new ArgumentException(PCResource.SettingsDoesNotBeNullMessage);
			}

			if (Settings.ClickonceUrl == null
				|| Settings.ClickonceUrl.Trim() == string.Empty)
			{
				throw new ArgumentException(PCResource.BadSettingsUrlMessage);
			}

			if (Settings.DestinationDirectory == null
				|| Settings.DestinationDirectory.Trim() == string.Empty)
			{
				throw new ArgumentException(PCResource.BadInstallationFolderMessage);
			}

			string urlRedirect = null;
			var deployManifest = DownloadDeployManifest(Settings.ClickonceUrl, out urlRedirect);
			if (urlRedirect != Settings.ClickonceUrl)
			{
				Settings.ClickonceUrl = urlRedirect;
			}
			if (deployManifest == null)
			{
				throw new Exception(PCResource.BadClickonceUrlMessage);
			}
			m_BgWorker.ReportProgress(0, string.Format("Version : {0}", deployManifest.AssemblyIdentity.Version));
			m_BgWorker.ReportProgress(0, string.Format("Application : {0}", deployManifest.AssemblyIdentity.Name));
			m_BgWorker.ReportProgress(0, PCResource.DeployFileLoadedMessage);
			var manifest = DownloadManifest(deployManifest, Settings.ClickonceUrl, out urlRedirect);
			m_BgWorker.ReportProgress(0, PCResource.ManifestFileLoadedMessage);
			m_BgWorker.ReportProgress(0, PCResource.StartingDownloadMessage);

			DownloadAll(deployManifest, manifest, Settings.ClickonceUrl, Settings.DestinationDirectory);
		}

		void m_BgWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
		{
			if (e.Error != null)
			{
				if (DownloadFailed != null)
				{
					DownloadFailed(this, new EventArgs<Exception>(e.Error));
				}
			}
			if (DownloadFinished != null)
			{
				DownloadFinished(this, EventArgs.Empty);
			}
		}

		void m_BgWorker_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
		{
			if (DownloadProgress != null)
			{
				DownloadProgress(this, new EventArgs<KeyValuePair<int, object>>(new KeyValuePair<int, object>(e.ProgressPercentage, e.UserState)));
			}
		}

		#endregion

		private DeployManifest DownloadDeployManifest(string url, out string urlRedirect)
		{
			var result = CrawlContent(url, out urlRedirect);
			DeployManifest manifest = null;

			using (var ms = new MemoryStream())
			{
				byte[] buffer = System.Text.Encoding.Default.GetBytes(result);
				ms.Write(buffer, 0, buffer.Length);
				ms.Seek(0, SeekOrigin.Begin);

				manifest = (DeployManifest)ManifestReader.ReadManifest(ms, false);
				ms.Close();
			}
			return manifest;
		}

		private Manifest DownloadManifest(DeployManifest deployManifest, string url, out string urlRedirect)
		{
			url = url.Split('?')[0]; // Remove all parameters
			string file = System.IO.Path.GetFileName(url);
			var rootUrl = url.Replace(file, "").TrimEnd('/').ToLower();
			string manifestUrl = string.Format("{0}/{1}", rootUrl, deployManifest.EntryPoint);
			manifestUrl = Uri.EscapeUriString(manifestUrl.Replace("\\", "/"));
			string manifestContent = CrawlContent(manifestUrl, out urlRedirect);

			Manifest manifest;

			using (var ms = new MemoryStream())
			{
				byte[] buffer = System.Text.Encoding.Default.GetBytes(manifestContent);
				ms.Write(buffer, 0, buffer.Length);
				ms.Seek(0, SeekOrigin.Begin);

				manifest = (AssemblyManifest)ManifestReader.ReadManifest(ms, false);
				ms.Close();
			}

			return manifest;
		}

		private void DownloadAll(DeployManifest deployManifest, Manifest manifest, string url, string directory)
		{
			string[] exetoken = deployManifest.EntryPoint.ToString().Split('\\');
			string versionFolder = deployManifest.AssemblyIdentity.Version;
			string folderPath = System.IO.Path.Combine(directory, versionFolder);
			System.IO.Directory.CreateDirectory(folderPath);

			var exeFileName = deployManifest.EntryPoint;
			Settings.Version = versionFolder;

			var token = url.Split('/');
			string path = url.Replace(token[token.Length - 1], "");

			string rootUrl = url;
			var total = manifest.AssemblyReferences.Count + manifest.FileReferences.Count;
			m_BgWorker.ReportProgress(0, string.Format(PCResource.TotalFileCountMessage, total));
			var i = 1;

			foreach (AssemblyReference ar in manifest.AssemblyReferences)
			{
				if (m_BgWorker.CancellationPending)
				{
					break;
				}

				if (ar.AssemblyIdentity.Name.Equals(deployManifest.EntryPoint.AssemblyIdentity.Name, StringComparison.CurrentCultureIgnoreCase))
				{
					var mainExeFileName = ((Microsoft.Build.Tasks.Deployment.ManifestUtilities.BaseReference)(ar)).TargetPath;
					Settings.ExeFileName = System.IO.Path.Combine(folderPath, mainExeFileName);
				}

				int progress = Convert.ToInt32(i * 100.0 / (total * 1.0));
				m_BgWorker.ReportProgress(0, string.Format(PCResource.FileDownloadMessage,ar.TargetPath));
				m_BgWorker.ReportProgress(-1, progress);
				i++;
				if (string.IsNullOrEmpty(ar.TargetPath))
				{
					continue;
				}

				CreateDirectory(folderPath, ar.TargetPath);

				string fileName = System.IO.Path.Combine(folderPath, ar.TargetPath);
				var applicationFileName = System.IO.Path.GetFileName(url);
				rootUrl = url.Replace(applicationFileName, "").Trim('/');
				rootUrl = string.Format("{0}/{1}.deploy", rootUrl, ar.TargetPath);
				rootUrl = rootUrl.Replace(@"\", "/");
				using (var fs = new System.IO.FileStream(fileName, System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.Write))
				{
					while (true)
					{
						if (m_BgWorker.CancellationPending)
						{
							break;
						}
						try
						{
							CrawlFile(rootUrl, fs, null, null);
							fs.Flush();
							fs.Close();
							break;
						}
						catch (Exception ex)
						{
							Console.WriteLine(ex.Message);
							if (rootUrl.IndexOf(".deploy") == -1)
							{
								break;
							}
							rootUrl = rootUrl.Replace(".deploy", "");
						}
					}
				}
			}

			foreach (FileReference file in manifest.FileReferences)
			{
				if (m_BgWorker.CancellationPending)
				{
					break;
				}

				int progress = Convert.ToInt32(i * 100.0 / (total * 1.0));
				m_BgWorker.ReportProgress(0, string.Format(PCResource.FileDownloadMessage , file.TargetPath));
				m_BgWorker.ReportProgress(-1, progress);
				i++;
				if (string.IsNullOrEmpty(file.TargetPath))
				{
					continue;
				}

				CreateDirectory(folderPath, file.TargetPath);

				string fileName = System.IO.Path.Combine(folderPath, file.TargetPath);
				var applicationFileName = System.IO.Path.GetFileName(url);
				rootUrl = url.Replace(applicationFileName, "").Trim('/');
				rootUrl = string.Format("{0}/{1}.deploy", rootUrl, file.TargetPath);
				rootUrl = rootUrl.Replace(@"\", "/");
				using (var fs = new System.IO.FileStream(fileName, System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.Write))
				{
					while (true)
					{
						if (m_BgWorker.CancellationPending)
						{
							break;
						}
						try
						{
							CrawlFile(rootUrl, fs, null, null);
							fs.Flush();
							fs.Close();
							break;
						}
						catch (Exception ex)
						{
							Console.WriteLine(ex.Message);
							if (rootUrl.IndexOf(".deploy") == -1)
							{
								break;
							}
							rootUrl = rootUrl.Replace(".deploy", "");
						}
					}
				}
			}
		}

		private string CrawlContent(string url, out string responseUri)
		{
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
			request.Method = "GET";
			request.UserAgent = "Mozilla/5.0 (compatible; PortableClickonce/1.0; +http://portableclickonce.codeplex.com/)";
			request.KeepAlive = true;
			request.Accept = "*/*";
			request.UseDefaultCredentials = true;
			request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
			request.MaximumAutomaticRedirections = 10;
			request.AllowAutoRedirect = true;

			HttpWebResponse response = null;
			responseUri = url;
			try
			{
				response = (HttpWebResponse)request.GetResponse();
				responseUri = response.ResponseUri.ToString();
			}
			catch (Exception ex)
			{
				if (response != null)
				{
					response.Close();
					response = null;
				}
				request = null;
				return string.Empty;
			}

			if (response.StatusCode != HttpStatusCode.OK)
			{
				throw new WebException(string.Format(PCResource.FileDownloadErrorMessage, response.StatusCode));
			}

			StringBuilder sb = null;
			if (response.ContentLength > 0)
			{
				sb = new StringBuilder((int)response.ContentLength);
			}
			else
			{
				sb = new StringBuilder();
			}
			using (Stream str = response.GetResponseStream())
			{
				using (StreamReader reader = new StreamReader(str, System.Text.Encoding.UTF8))
				{
					int bufferSize = 1024;
					char[] buffer = new char[bufferSize];
					int pos = 0;
					while ((pos = reader.Read(buffer, 0, bufferSize)) > 0)
					{
						sb.Append(buffer, 0, pos);
					}
					reader.Close();
				}
			}

			if (response != null)
			{
				response.Close();
				response = null;
			}
			request = null;
			return sb.ToString();
		}

		/// <summary>
		/// Gets the content of the specified webpage.
		/// </summary>
		/// <param name="url">The url to get.</param>
		/// <param name="output">The output.</param>
		/// <returns></returns>
		private void CrawlFile(string url, Stream output, string user, string password)
		{
			var request = (HttpWebRequest)WebRequest.Create(url);
			request.UserAgent = "Mozilla/5.0 (compatible; PortableClickonce/1.0; +http://portableclickonce.codeplex.com/)";
			request.Method = "GET";
			request.KeepAlive = true;
			request.Accept = @"*/*";
			request.UseDefaultCredentials = true;
			request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
			request.MaximumAutomaticRedirections = 10;
			request.AllowAutoRedirect = true;

			HttpWebResponse response = null;
			try
			{
				response = (HttpWebResponse)request.GetResponse();
			}
			catch
			{
				if (response != null)
				{
					response.Close();
					response = null;
				}
				request = null;
				throw;
			}

			if (response.StatusCode != HttpStatusCode.OK)
			{
				throw new WebException(string.Format(PCResource.FileDownloadErrorMessage, response.StatusCode));
			}

			m_BgWorker.ReportProgress(-2, 0);
			using (Stream str = response.GetResponseStream())
			{
				long contentLenght = response.ContentLength;
				int bufferSize = 1024;
				// long old = 0;
				byte[] buffer = new byte[bufferSize];
				int pos = 0;

				string oldText = string.Empty;

				var i = 0;
				while ((pos = str.Read(buffer, 0, bufferSize)) > 0)
				{
					output.Write(buffer, 0, pos);
					int progress = Math.Min(100, Convert.ToInt32(i * buffer.Length * 100.0 / (contentLenght * 1.0)));
					m_BgWorker.ReportProgress(-2, progress);
					i++;
				}
				str.Close();
			}

			if (response != null)
			{
				response.Close();
				response = null;
			}
			request = null;
		}


		private void CreateDirectory(string root, string path)
		{
			path = path.Trim('\\');
			// Il existe un sub folder
			if (path.IndexOf(@"\") != -1)
			{
				string[] folders = path.Split('\\');
				for (int i = 0; i < folders.Length - 1; i++)
				{
					string folder = folders[i];
					root = root + @"\" + folder;
					if (!System.IO.Directory.Exists(root))
					{
						System.IO.Directory.CreateDirectory(root);
					}
				}
			}

		}
	}
}
