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
            StorageModeForm f = new StorageModeForm(this);
            f.Show();
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            base.setConfigValue("StoreHands", "0");
            ArchiveForm f = new ArchiveForm(this);
            f.Show();
            this.Hide();
        }
    }
}
