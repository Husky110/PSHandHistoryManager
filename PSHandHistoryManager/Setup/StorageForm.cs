using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PSHandHistoryManager.Setup
{
    public partial class StorageForm : HandManagerForm
    {
        public StorageForm(HandManagerForm prevForm)
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
            base.setConfigValue("StoreHands", "1");
            string workingDir = ConfigurationManager.AppSettings["WorkingDirectory"];
            string storageDir = Directory.GetParent(workingDir) + "\\storage\\";
            if (Directory.Exists(storageDir) == false)
            {
                Directory.CreateDirectory(storageDir);
            }
            base.setConfigValue("StorageFolder", storageDir);
            SetupHUDForm f = new SetupHUDForm(this);
            f.Show();
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            base.setConfigValue("StoreHands", "0");
            SetupHUDForm f = new SetupHUDForm(this);
            f.Show();
            this.Hide();
        }
    }
}
