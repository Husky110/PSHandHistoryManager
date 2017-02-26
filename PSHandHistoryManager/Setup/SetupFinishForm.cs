using System;
using System.Security.Cryptography;
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
            string fakeplayername = "";
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] textToHash = Encoding.Default.GetBytes(new DateTime().ToLongTimeString());
            byte[] result = md5.ComputeHash(textToHash);
            fakeplayername = BitConverter.ToString(result).Replace("-", "‌​").ToLower();
            base.setConfigValue("FakeplayerName", fakeplayername);
            textBox1.Text = fakeplayername;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            base.showPreviousForm();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            base.setConfigValue("Setup", "0");
            MainForm.instance.Close();
            this.Close();
        }

    }
}
