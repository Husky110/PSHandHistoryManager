using System;
using System.Windows.Forms;
using System.IO;
using System.Resources;

namespace PSHandHistoryManager.Setup
{
    public partial class PokerStarsFormInitial : HandManagerForm
    {

        public PokerStarsFormInitial(SetupInitialForm f)
        {
            InitializeComponent();
            base.initializeHandManagerForm(f, this.GetType());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.previousForm.Show();
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //TODO: automatic setup for PokerStars
            MessageBox.Show("This feature is not implemented yet!");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MessageBox.Show(this.resourceManager.GetString("information.emptyfolder"), "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            bool validation = false;
            while(validation == false)
            {
                FolderBrowserDialog folder = new FolderBrowserDialog();
                folder.ShowNewFolderButton = true;
                folder.ShowDialog();
                if(folder.SelectedPath != String.Empty) { 
                    validation = this.valiateFolder(folder.SelectedPath);
                }
                else
                {
                    break;
                }
                if (validation == false)
                {
                    MessageBox.Show(this.resourceManager.GetString("error.foldernotempty"), "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    HandManagerForm f = new PokerStarsFormFolderManual(this, folder.SelectedPath);
                    this.Hide();
                    f.Show();
                }
            }
        }

        private bool valiateFolder(string path)
        {
            if(Directory.GetFiles(path).Length > 0 || Directory.GetDirectories(path).Length > 0)
            {
                return false;
            }
            return true;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            base.showPreviousForm();
        }
    }
}
