using System;
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
    public partial class SetupHUDForm : HandManagerForm
    {
        public SetupHUDForm(HandManagerForm prevForm)
        {
            InitializeComponent();
            base.initializeHandManagerForm(prevForm, this.GetType());
            txt_source.Text = ConfigurationManager.AppSettings["SourceFolder"];
            txt_Output.Text = ConfigurationManager.AppSettings["OutputDirectory"];
            txt_unsupported.Text = ConfigurationManager.AppSettings["UnsupportedLanguageDirectory"];
            this.Refresh();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            base.showPreviousForm();
        }
        
        private void button2_Click_1(object sender, EventArgs e)
        {
            if(checkBox1.Checked == true)
            {
                MessageBox.Show(this.resourceManager.GetString("information.HUDPath"), "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                OpenFileDialog file = new OpenFileDialog();
                file.Filter = "Applications (*.exe)|*.exe";
                file.ShowDialog();
                if(String.IsNullOrEmpty(file.FileName))
                {
                    return;
                }
                else
                {
                    base.setConfigValue("AutoRunHUD", "1");
                    base.setConfigValue("HUDPath", file.FileName);
                }
            }
            else
            {
                base.setConfigValue("AutoRunHUD", "0");
            }

            AutoRunProcessForm f = new AutoRunProcessForm(this);
            f.Show();
            this.Hide();
        }
    }
}
