using System;
using System.IO;
using System.Configuration;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PSHandHistoryManager.Setup
{
    public partial class ArchiveForm : HandManagerForm
    {
        public ArchiveForm(HandManagerForm prevForm)
        {
            InitializeComponent();
            base.initializeHandManagerForm(prevForm);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            base.showPreviousForm();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            base.setConfigValue("ArchiveUnprocessedHands", "1");
            string workingDir = ConfigurationManager.AppSettings["WorkingDirectory"];
            string archiveDir = Directory.GetParent(workingDir) + "\\archive\\";
            if(Directory.Exists(archiveDir) == false)
            {
                Directory.CreateDirectory(archiveDir);
            }
            base.setConfigValue("ArchiveHandsFolder", archiveDir);
            StorageForm f = new StorageForm(this);
            f.Show();
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            base.setConfigValue("ArchiveUnprocessedHands", "0");
            StorageForm f = new StorageForm(this);
            f.Show();
            this.Hide();
        }
    }
}
