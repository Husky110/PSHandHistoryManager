﻿using System;
using System.Windows.Forms;
using System.IO;
using System.Configuration;

namespace PSHandHistoryManager.Setup
{
    public partial class PokerStarsFormFolderManual : HandManagerForm
    {
        // TODO: find a solution for multiple PS-Users on the same computer -> right now we can only provide 1 user per PC

        private string selectedFolder = "";
        public PokerStarsFormFolderManual(HandManagerForm prevForm, string folderPath)
        {
            base.initializeHandManagerForm(prevForm, this.GetType());
            this.selectedFolder = folderPath;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            base.showPreviousForm();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string[] folders = Directory.GetDirectories(this.selectedFolder);
            if(folders.Length == 0)
            {
                MessageBox.Show(this.resourceManager.GetString("error.FolderNotFound"), "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                base.setConfigValue("SourceFolder", folders[0] + "\\");
                string workingDirectory = Directory.GetParent(folders[0]).FullName + "\\PSHandManager";
                Directory.CreateDirectory(workingDirectory);
                base.setConfigValue("WorkingDirectory", workingDirectory + "\\unprocessed");
                base.setConfigValue("OutputDirectory", workingDirectory + "\\processed");
                string hudMessage = string.Format(this.resourceManager.GetString("information.configureHUD"), workingDirectory + "\\processed");
                MessageBox.Show(hudMessage, "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                StorageForm f = new StorageForm(this);
                f.Show();
                this.Hide();
            }
        }
    }
}
