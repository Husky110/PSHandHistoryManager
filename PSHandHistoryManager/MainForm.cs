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
using System.Threading;

namespace PSHandHistoryManager
{
    public partial class MainForm : HandManagerForm
    {
        public static MainForm instance;
        private Task handProcessorTask;

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
            Manager hp = new Manager(this.pathToAppConfig);
            Task t = new Task(() => hp.run());
            t.Start();
            this.handProcessorTask = t;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Manager.shutdown = true;
            while(this.handProcessorTask.IsCompleted == false)
            {
                Thread.Sleep(100); // wait 100ms
            }
            this.Close();
        }
    }
}
