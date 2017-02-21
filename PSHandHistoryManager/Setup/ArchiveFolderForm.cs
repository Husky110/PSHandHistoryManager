using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PSHandHistoryManager.Setup
{
    public partial class ArchiveFolderForm : HandManagerForm
    {
        string archiveFolder = "";
        public ArchiveFolderForm(HandManagerForm prevForm)
        {
            InitializeComponent();
            base.initializeHandManagerForm(prevForm, this.GetType());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folder = new FolderBrowserDialog();
            folder.ShowNewFolderButton = true;
            folder.ShowDialog();
            this.archiveFolder = folder.SelectedPath;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            base.showPreviousForm();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if(this.archiveFolder == "")
            {
                MessageBox.Show(this.resourceManager.GetString("error.NoFolder"), "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                base.setConfigValue("ArchiveHandsFolder", this.archiveFolder);
                AutoRunPSForm f = new AutoRunPSForm(this);
                f.Show();
                this.Hide();
            }
        }
    }
}
