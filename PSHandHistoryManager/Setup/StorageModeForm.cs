using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration;

namespace PSHandHistoryManager.Setup
{
    public partial class StorageModeForm : HandManagerForm
    {
        private string storageFolder = "";
        public StorageModeForm(HandManagerForm f)
        {
            InitializeComponent();
            base.initializeHandManagerForm(f,this.GetType());
            this.initializeComboBoxForStorageMethods();

        }

        private void initializeComboBoxForStorageMethods()
        {
            comboBox1.Items.Add(this.resourceManager.GetString("storageMode.PS"));
            comboBox1.Items.Add(this.resourceManager.GetString("storageMode.AllinOne"));
            comboBox1.SelectedIndex = Convert.ToInt16(ConfigurationManager.AppSettings["StorageMode"]);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            base.showPreviousForm();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog f = new FolderBrowserDialog();
            f.ShowNewFolderButton = true;
            f.ShowDialog();
            this.storageFolder = f.SelectedPath + "\\";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if(this.storageFolder == "")
            {
                MessageBox.Show(this.resourceManager.GetString("error.NoFolder"), "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                base.setConfigValue("StorageMode", comboBox1.SelectedIndex.ToString());
                base.setConfigValue("StorageFolder", this.storageFolder);
                ArchiveForm f = new ArchiveForm(this);
                f.Show();
                this.Hide();
            }
        }
    }
}
