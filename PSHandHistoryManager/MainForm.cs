using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using PSHandManagerLib;

namespace PSHandHistoryManager
{
    public partial class MainForm : HandManagerForm
    {
        public static MainForm instance;

        private string pathToAppConfig = "";
        public MainForm(string configFilePath)
        {
            InitializeComponent();
            base.initializeHandManagerForm(this.GetType());
            MainForm.instance = this;
            this.pathToAppConfig = configFilePath;
        }

        private void Main_Shown(object sender, EventArgs e)
        {
            if(ConfigurationManager.AppSettings["Setup"] == "1")
            {
                this.Hide();
                Setup.SetupInitialForm f = new Setup.SetupInitialForm();
                f.Show();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            HandProcessor hp = new HandProcessor(this.pathToAppConfig);
            Task t = new Task(() => hp.run());
            t.Start();
        }
    }
}
