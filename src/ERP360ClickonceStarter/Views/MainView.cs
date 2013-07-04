using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PortableClickonce.Views
{
	public partial class MainView : Form
	{
		public MainView()
			: this(new Models.ClickonceSettings())
		{
		}

		public MainView(Models.ClickonceSettings settings)
		{
			InitializeComponent();
			this.Settings = settings;
			this.DownloadService = new Services.DownloaderService();
			this.DownloadService.DownloadStarted += (s, arg) =>
				{
					WriteLog(PCResource.StartDownloadLogMessage);
				};
			this.DownloadService.DownloadProgress += new EventHandler<Services.EventArgs<KeyValuePair<int, object>>>(DownloadService_DownloadProgress);
			this.DownloadService.DownloadFinished += new EventHandler<EventArgs>(DownloadService_DownloadFinished);
			this.DownloadService.DownloadFailed += (s, arg) =>
				{
					WriteLog(arg.Data.ToString());
					uxCancelButton.Text = PCResource.CloseButtonText;
				};

			// Localization
			uxCancelButton.Text = PCResource.CancelButtonText;
			uxChooseDirectoryButton.Text = PCResource.ChooseDirectoryButtonText;
			uxClickonceUrlLabel.Text = PCResource.ClickonceUrlLabelText;
			uxDownloadButton.Text = PCResource.DownloadButtonText;
			uxLocalInstallationFolderLabel.Text = PCResource.LocalInstallationFolderLabelText;
			uxLogLabel.Text = PCResource.LogLabelText;
		}

		protected Models.ClickonceSettings Settings { get; set; }
		protected Services.DownloaderService DownloadService { get; set; }

		public bool CancelDownload { get; set; }

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			uxUrlTextBox.DataBindings.Add("Text", Settings, "ClickonceUrl", true, DataSourceUpdateMode.OnPropertyChanged);
			uxDirectoryDestinationTextBox.DataBindings.Add("Text", Settings, "DestinationDirectory", true, DataSourceUpdateMode.OnPropertyChanged);
		}

		private void uxDownloadButton_Click(object sender, EventArgs e)
		{
			try
			{
				DownloadService.StartFullDownload(Settings);
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message, PCResource.MsgBoxWarningTitle, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
				return;
			}
			uxDownloadButton.Enabled = false;
			uxCancelButton.Text = "Annuler";
			uxProgressBar.Maximum = 100;
			uxProgressBar.Value = 0;
			uxProgressBar.Style = ProgressBarStyle.Blocks;

			uxSubProgressBar.Maximum = 100;
			uxSubProgressBar.Value = 0;
			uxSubProgressBar.Style = ProgressBarStyle.Blocks;
		}

		public void WriteLog(string content)
		{
			uxLogTextBox.Text = string.Format("{0}{1}{2}", content, System.Environment.NewLine, uxLogTextBox.Text);
		}

		void DownloadService_DownloadProgress(object sender, Services.EventArgs<KeyValuePair<int, object>> e)
		{
			if (e.Data.Key == 0)
			{
				WriteLog(e.Data.Value.ToString());
			}
			else if (e.Data.Key == -1)
			{
				uxProgressBar.Value = int.Parse(e.Data.Value.ToString());
			}
			else if (e.Data.Key == -2)
			{
				uxSubProgressBar.Value = int.Parse(e.Data.Value.ToString());
			}
		}

		void DownloadService_DownloadFinished(object sender, EventArgs e)
		{
			uxProgressBar.Value = 0;
			uxSubProgressBar.Value = 0;
			uxCancelButton.Text = PCResource.LaunchButtongText;
		}

		private void uxCancelButton_Click(object sender, EventArgs e)
		{
			DownloadService.Cancel();
			if (uxCancelButton.Text != PCResource.LaunchButtongText)
			{
				CancelDownload = true;
			}
			this.Close();
		}

	}
}
