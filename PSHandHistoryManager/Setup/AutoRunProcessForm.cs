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
    public partial class AutoRunProcessForm : HandManagerForm
    {
        public AutoRunProcessForm(HandManagerForm prevForm)
        {
            InitializeComponent();
            base.initializeHandManagerForm(prevForm, this.GetType());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            base.showPreviousForm();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            base.setConfigValue("AutoStartProcessing", "1");
            SetupFinishForm f = new SetupFinishForm(this);
            f.Show();
            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            base.setConfigValue("AutoStartProcessing", "0");
            SetupFinishForm f = new SetupFinishForm(this);
            f.Show();
            this.Hide();
        }
    }
}
