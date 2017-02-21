using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;
using System.Globalization;
using System.Configuration;
using System.Linq;
using System.Resources;

namespace PSHandHistoryManager.Setup
{
    public partial class SetupInitialForm : HandManagerForm
    {
        private Dictionary<String, String> languages;
        public SetupInitialForm()
        {
            InitializeComponent();
            InitializeLanguages();
            base.initializeHandManagerForm(this.GetType());
        }

        private void InitializeLanguages()
        {
            comboBox1.Items.Clear();
            this.languages = Helpers.FormHelper.GetSupportedCultures();
            foreach (string lang in this.languages.Keys)
            {
                comboBox1.Items.Add(lang);

            }
            if(ConfigurationManager.AppSettings["Language"] != "")
            {
                string key = this.languages.FirstOrDefault(x => x.Value == ConfigurationManager.AppSettings["Language"]).Key;
                comboBox1.SelectedIndex = comboBox1.Items.IndexOf(key);
            }
            else
            {
                comboBox1.SelectedIndex = 0;
            }
        }

        private void ChangeLanguage(int index)
        {
            string lang = this.languages[comboBox1.SelectedItem.ToString()];
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(lang);
            this.Controls.Clear();
            this.InitializeComponent();
            this.InitializeLanguages();
            comboBox1.SelectedItem = comboBox1.Items[index];

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            base.setConfigValue("Language", this.languages[comboBox1.SelectedItem.ToString()]);
            if (MessageBox.Show(this.resourceManager.GetString("warning.closePS"), "", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
            {
                HandManagerForm f = new PokerStarsFormInitial(this);
                f.Show();
                this.Hide();
            }
        }

        private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            this.ChangeLanguage(comboBox1.SelectedIndex);
        }
    }
}
