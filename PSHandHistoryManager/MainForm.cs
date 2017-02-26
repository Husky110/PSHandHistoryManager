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
using System.IO;
using System.Diagnostics;

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
            this.textBox1.Text = ConfigurationManager.AppSettings["FakeplayerName"];
            this.textBox1.Enabled = false;
            if (ConfigurationManager.AppSettings["AutoStartProcessing"] == "1")
            {
                this.startWork();
                button1.Enabled = false;
            }
            else
            {
                button4.Enabled = false;
            }
            if (ConfigurationManager.AppSettings["AutoRunPS"] == "1")
            {
                Process[] pname = Process.GetProcessesByName("PokerStars");
                if(pname.Length == 0)
                { 
                    Process.Start(ConfigurationManager.AppSettings["PSPath"]);
                }
            }
            if(ConfigurationManager.AppSettings["AutoRunHUD"] == "1")
            {
                Process[] pname = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(ConfigurationManager.AppSettings["HUDPath"]));
                if(pname.Length == 0)
                {
                    Process.Start(new ProcessStartInfo() {FileName = ConfigurationManager.AppSettings["HUDPath"], WorkingDirectory = Path.GetDirectoryName(ConfigurationManager.AppSettings["HUDPath"]) });
                }
            }
            this.Refresh();
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

        private void startWork()
        {
            Manager hp = new Manager(Application.StartupPath);
            Task t = new Task(() => hp.run());
            t.Start();
            this.handProcessorTask = t;
            this.button3.Enabled = false;
            this.Refresh();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            startWork();
        }

        private void stopWork()
        {
            this.button4.Enabled = false;
            this.Refresh();
            Manager.shutdown = true;
            while (this.handProcessorTask != null && this.handProcessorTask.IsCompleted == false)
            {
                Thread.Sleep(10); // wait 10ms
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            button5.Text = "Exiting...";
            foreach(Control c in this.Controls)
            {
                c.Enabled = false;
            }
            this.stopWork();
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(Manager.runningTasks.Count > 0)
            {
                if(MessageBox.Show(this.resourceManager.GetString("information.ConfigureForm"), "", MessageBoxButtons.OK, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    this.stopWork();
                }
            }
            ConfigurationForm f = new ConfigurationForm(this);
            f.Show();
            this.Hide();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.stopWork();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MessageBox.Show(this.resourceManager.GetString("information.ImportForm"), "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
