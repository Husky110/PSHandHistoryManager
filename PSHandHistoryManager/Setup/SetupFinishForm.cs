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
    public partial class SetupFinishForm : HandManagerForm
    {
        public SetupFinishForm(HandManagerForm prevForm)
        {
            InitializeComponent();
            this.initializeHandManagerForm(prevForm);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            base.showPreviousForm();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            base.setConfigValue("Setup", "0");
            MainForm.instance.Show();
            this.Close();
        }

    }
}
