using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Build.Tasks.Deployment.ManifestUtilities;
using System.IO;
using System.Windows.Forms;

namespace PortableClickonce
{
	public class Program
	{
		static void Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
			Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(OnApplicationThreadException);
			Application.ThreadExit += new EventHandler(OnApplicationThreadExit);

			var settings = Services.SettingsManager.Load();
			var downloaderService = new Services.DownloaderService();

			var state = downloaderService.IsLatestVersion(settings);
			if (state == -1)
			{
				var form = new Views.MainView(settings);
				Application.Run(form);
				if (form.CancelDownload)
				{
					return;
				}
			}
			else if (state == -2)
			{
				MessageBox.Show(PCResource.UnknownVersionMessage, PCResource.MsgBoxWarningTitle,  MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}

			settings.LatestUse = DateTime.Now;
			Services.SettingsManager.SaveSettings(settings);

			if (settings.FullExeFileName == null)
			{
				return;
			}

			var psi = new System.Diagnostics.ProcessStartInfo(settings.FullExeFileName);
			psi.RedirectStandardError = true;
			psi.UseShellExecute = false;
			psi.CreateNoWindow = false;
			psi.ErrorDialog = false;

			var p = new System.Diagnostics.Process();
			p.ErrorDataReceived += (s, arg) =>
				{
					MessageBox.Show(arg.Data, PCResource.MsgBoxErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
				};

			p.StartInfo = psi;

			try
			{
				p.Start();
				settings.LastRunSuccess = true;
			}
			catch
			{
				settings.LastRunSuccess = false;
			}
			Services.SettingsManager.SaveSettings(settings);
		}

		static void OnApplicationThreadExit(object sender, EventArgs e)
		{
			// AppDomain.CurrentDomain.UnhandledException -= new UnhandledExceptionEventHandler(AppDomainUnhandledException);
			Application.ThreadException -= new System.Threading.ThreadExceptionEventHandler(OnApplicationThreadException);
			// settings.LastRunSuccess = true;
			// SettingsManager.SaveSettings(settings);
		}

		static void OnApplicationThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
		{
		}



	}
}
