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
    public partial class AutoRunHUDPathForm : HandManagerForm
    {
        private string hudPath = "";
        public AutoRunHUDPathForm(HandManagerForm prevForm)
        {
            InitializeComponent();
            base.initializeHandManagerForm(prevForm, this.GetType());
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog();
            file.Filter = "Applications (*.exe)|*.exe";
            file.ShowDialog();
            this.hudPath = file.FileName;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (this.hudPath == "")
            {
                MessageBox.Show(base.resourceManager.GetString("error.NoFile"), "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                base.setConfigValue("HUDPath", this.hudPath);
                AutoRunProcessForm f = new AutoRunProcessForm(this);
                f.Show();
                this.Hide();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            base.showPreviousForm();
        }
    }
}
