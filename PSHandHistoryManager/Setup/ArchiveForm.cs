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
            base.setConfigValue("ArchivePSHandFiles", "1");
            ArchiveFolderForm f = new ArchiveFolderForm(this);
            f.Show();
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            AutoRunPSForm f = new AutoRunPSForm(this);
            f.Show();
            this.Hide();
        }
    }
}
