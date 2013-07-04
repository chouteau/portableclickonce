namespace PortableClickonce.Views
{
	partial class MainView
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.uxUrlTextBox = new System.Windows.Forms.TextBox();
			this.uxClickonceUrlLabel = new System.Windows.Forms.Label();
			this.uxDirectoryDestinationTextBox = new System.Windows.Forms.TextBox();
			this.uxLocalInstallationFolderLabel = new System.Windows.Forms.Label();
			this.uxProgressBar = new System.Windows.Forms.ProgressBar();
			this.uxSubProgressBar = new System.Windows.Forms.ProgressBar();
			this.uxCancelButton = new System.Windows.Forms.Button();
			this.uxDownloadButton = new System.Windows.Forms.Button();
			this.uxChooseDirectoryButton = new System.Windows.Forms.Button();
			this.uxLogTextBox = new System.Windows.Forms.TextBox();
			this.uxLogLabel = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// uxUrlTextBox
			// 
			this.uxUrlTextBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
			this.uxUrlTextBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.HistoryList;
			this.uxUrlTextBox.Location = new System.Drawing.Point(12, 37);
			this.uxUrlTextBox.Name = "uxUrlTextBox";
			this.uxUrlTextBox.Size = new System.Drawing.Size(535, 20);
			this.uxUrlTextBox.TabIndex = 0;
			// 
			// uxClickonceUrlLabel
			// 
			this.uxClickonceUrlLabel.AutoSize = true;
			this.uxClickonceUrlLabel.Location = new System.Drawing.Point(13, 18);
			this.uxClickonceUrlLabel.Name = "uxClickonceUrlLabel";
			this.uxClickonceUrlLabel.Size = new System.Drawing.Size(294, 13);
			this.uxClickonceUrlLabel.TabIndex = 1;
			this.uxClickonceUrlLabel.Text = "Adresse du logiciel (exemple : http://start.erp360.net/demo) :";
			// 
			// uxDirectoryDestinationTextBox
			// 
			this.uxDirectoryDestinationTextBox.Location = new System.Drawing.Point(12, 89);
			this.uxDirectoryDestinationTextBox.Name = "uxDirectoryDestinationTextBox";
			this.uxDirectoryDestinationTextBox.Size = new System.Drawing.Size(454, 20);
			this.uxDirectoryDestinationTextBox.TabIndex = 0;
			// 
			// uxLocalInstallationFolderLabel
			// 
			this.uxLocalInstallationFolderLabel.AutoSize = true;
			this.uxLocalInstallationFolderLabel.Location = new System.Drawing.Point(13, 70);
			this.uxLocalInstallationFolderLabel.Name = "uxLocalInstallationFolderLabel";
			this.uxLocalInstallationFolderLabel.Size = new System.Drawing.Size(173, 13);
			this.uxLocalInstallationFolderLabel.TabIndex = 1;
			this.uxLocalInstallationFolderLabel.Text = "Chemin de destination des fichiers :";
			// 
			// uxProgressBar
			// 
			this.uxProgressBar.Location = new System.Drawing.Point(12, 131);
			this.uxProgressBar.Name = "uxProgressBar";
			this.uxProgressBar.Size = new System.Drawing.Size(535, 23);
			this.uxProgressBar.TabIndex = 2;
			// 
			// uxSubProgressBar
			// 
			this.uxSubProgressBar.Location = new System.Drawing.Point(13, 170);
			this.uxSubProgressBar.Name = "uxSubProgressBar";
			this.uxSubProgressBar.Size = new System.Drawing.Size(534, 23);
			this.uxSubProgressBar.TabIndex = 3;
			// 
			// uxCancelButton
			// 
			this.uxCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.uxCancelButton.Location = new System.Drawing.Point(472, 335);
			this.uxCancelButton.Name = "uxCancelButton";
			this.uxCancelButton.Size = new System.Drawing.Size(75, 23);
			this.uxCancelButton.TabIndex = 4;
			this.uxCancelButton.Text = "Fermer";
			this.uxCancelButton.UseVisualStyleBackColor = true;
			this.uxCancelButton.Click += new System.EventHandler(this.uxCancelButton_Click);
			// 
			// uxDownloadButton
			// 
			this.uxDownloadButton.Location = new System.Drawing.Point(472, 306);
			this.uxDownloadButton.Name = "uxDownloadButton";
			this.uxDownloadButton.Size = new System.Drawing.Size(75, 23);
			this.uxDownloadButton.TabIndex = 5;
			this.uxDownloadButton.Text = "Charger";
			this.uxDownloadButton.UseVisualStyleBackColor = true;
			this.uxDownloadButton.Click += new System.EventHandler(this.uxDownloadButton_Click);
			// 
			// uxChooseDirectoryButton
			// 
			this.uxChooseDirectoryButton.Location = new System.Drawing.Point(472, 87);
			this.uxChooseDirectoryButton.Name = "uxChooseDirectoryButton";
			this.uxChooseDirectoryButton.Size = new System.Drawing.Size(75, 23);
			this.uxChooseDirectoryButton.TabIndex = 4;
			this.uxChooseDirectoryButton.Text = "Choisir...";
			this.uxChooseDirectoryButton.UseVisualStyleBackColor = true;
			// 
			// uxLogTextBox
			// 
			this.uxLogTextBox.Location = new System.Drawing.Point(13, 224);
			this.uxLogTextBox.Multiline = true;
			this.uxLogTextBox.Name = "uxLogTextBox";
			this.uxLogTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.uxLogTextBox.Size = new System.Drawing.Size(453, 134);
			this.uxLogTextBox.TabIndex = 0;
			// 
			// uxLogLabel
			// 
			this.uxLogLabel.AutoSize = true;
			this.uxLogLabel.Location = new System.Drawing.Point(13, 208);
			this.uxLogLabel.Name = "uxLogLabel";
			this.uxLogLabel.Size = new System.Drawing.Size(75, 13);
			this.uxLogLabel.TabIndex = 1;
			this.uxLogLabel.Text = "Diagnostique :";
			// 
			// MainView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.uxCancelButton;
			this.ClientSize = new System.Drawing.Size(559, 370);
			this.Controls.Add(this.uxDownloadButton);
			this.Controls.Add(this.uxChooseDirectoryButton);
			this.Controls.Add(this.uxCancelButton);
			this.Controls.Add(this.uxSubProgressBar);
			this.Controls.Add(this.uxProgressBar);
			this.Controls.Add(this.uxLocalInstallationFolderLabel);
			this.Controls.Add(this.uxLogLabel);
			this.Controls.Add(this.uxClickonceUrlLabel);
			this.Controls.Add(this.uxLogTextBox);
			this.Controls.Add(this.uxDirectoryDestinationTextBox);
			this.Controls.Add(this.uxUrlTextBox);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "MainView";
			this.Text = "Portable Clickonce";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox uxUrlTextBox;
		private System.Windows.Forms.Label uxClickonceUrlLabel;
		private System.Windows.Forms.TextBox uxDirectoryDestinationTextBox;
		private System.Windows.Forms.Label uxLocalInstallationFolderLabel;
		private System.Windows.Forms.ProgressBar uxProgressBar;
		private System.Windows.Forms.ProgressBar uxSubProgressBar;
		private System.Windows.Forms.Button uxCancelButton;
		private System.Windows.Forms.Button uxDownloadButton;
		private System.Windows.Forms.Button uxChooseDirectoryButton;
		private System.Windows.Forms.TextBox uxLogTextBox;
		private System.Windows.Forms.Label uxLogLabel;
	}
}