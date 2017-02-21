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

namespace PSHandHistoryManager
{
    public partial class MainForm : HandManagerForm
    {
        public static MainForm instance;
        public MainForm()
        {
            InitializeComponent();
            base.initializeHandManagerForm(this.GetType());
            MainForm.instance = this;
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
    }
}
