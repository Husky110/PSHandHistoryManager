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
    public partial class AutoRunHUDForm : HandManagerForm
    {
        public AutoRunHUDForm(HandManagerForm prevForm)
        {
            InitializeComponent();
            base.initializeHandManagerForm(prevForm);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            base.showPreviousForm();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            base.setConfigValue("AutoRunHUD", "1");
            AutoRunHUDPathForm f = new AutoRunHUDPathForm(this);
            f.Show();
            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            base.setConfigValue("AutoRunHUD", "0");
            AutoRunProcessForm f = new AutoRunProcessForm(this);
            f.Show();
            this.Hide();
        }
    }
}
