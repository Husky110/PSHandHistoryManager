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
    public partial class AutoRunPSForm : HandManagerForm
    {
        public AutoRunPSForm(HandManagerForm prevForm)
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
            base.setConfigValue("AutoRunPS", "0");
            this.showAutoRunHUDForm();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            base.setConfigValue("AutoRunPS", "1");
            base.setConfigValue("PSPath", Helpers.FormHelper.getPokerStarsPath());
            this.showAutoRunHUDForm();
        }

        private void showAutoRunHUDForm()
        {
            AutoRunHUDForm f = new AutoRunHUDForm(this);
            f.Show();
            this.Hide();
        }
    }
}
