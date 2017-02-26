using System;
using System.Windows.Forms;
using System.IO;
using System.Resources;
using System.Collections.Generic;
using System.Text;

namespace PSHandHistoryManager.Setup
{
    public partial class PokerStarsFormInitial : HandManagerForm
    {
        public PokerStarsFormInitial(SetupInitialForm f)
        {
            InitializeComponent();
            base.initializeHandManagerForm(f, this.GetType());
            button3.Enabled = false;
            button4.Enabled = false;
            this.Refresh();
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

        private void button2_Click_1(object sender, EventArgs e)
        {
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string optionfilepath = "";
            string[] directoriesInlocalAppData = Directory.GetDirectories(localAppData);
            foreach(string folder in directoriesInlocalAppData)
            {
                if (folder.ToLower().Contains("pokerstars"))
                {
                    string[] filesInFolder = Directory.GetFiles(folder);
                    bool foundUserINI = false;
                    foreach (string file in filesInFolder)
                    {
                        if(Path.GetFileName(file) == "user.ini")
                        {
                            foundUserINI = true;
                            optionfilepath = file;
                            break;
                        }
                    }
                    if (foundUserINI)
                    {
                        break;
                    }
                }
            }
            if(optionfilepath == "")
            {
                MessageBox.Show(this.resourceManager.GetString("error.NoINI"), "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            List<String> optionFileContent = new List<string>();
            using(FileStream fs = new FileStream(optionfilepath, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                using(StreamReader sr = new StreamReader(fs, new UTF8Encoding()))
                {
                    while(sr.Peek() > -1)
                    {
                        optionFileContent.Add(sr.ReadLine());
                    }
                }
            }
            string saveMyHandsPath = "";
            for (int x = 0; x < optionFileContent.Count; x++)
            {
                string line = optionFileContent[x];
                if (line.StartsWith("SaveMyHandsPath=")) // get source-directory -> either from Option or from PipeOption and PipeOption > Option
                {
                    saveMyHandsPath = line.Replace("SaveMyHandsPath=", "");
                }
                if (line.StartsWith("SaveMyHands=")) //make sure hands are beein saved
                {
                    line = "SaveMyHands=1";
                    optionFileContent[x] = line;
                }
                if (line.StartsWith("HHLocale=")) //try to set english as filelanguage
                {
                    line = "HHLocale=0";
                    optionFileContent[x] = line;
                }
            }
            if(saveMyHandsPath == "")
            {
                MessageBox.Show(this.resourceManager.GetString("error.PSNoHH"), "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else
            {
                base.setConfigValue("SourceFolder", saveMyHandsPath);
                button3.Enabled = true;
                this.Refresh();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            MessageBox.Show(this.resourceManager.GetString("information.setupFolder"), "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            folderBrowser.ShowNewFolderButton = true;
            folderBrowser.ShowDialog();
            string managerEnvironment = folderBrowser.SelectedPath + "\\";

            string workingFolder = managerEnvironment + "unprocessed\\";
            if(Directory.Exists(workingFolder) == false)
            {
                Directory.CreateDirectory(workingFolder);
            }
            base.setConfigValue("WorkingDirectory", workingFolder);

            string outputDirectory = managerEnvironment + "processed\\";
            if (Directory.Exists(outputDirectory) == false)
            {
                Directory.CreateDirectory(outputDirectory);
            }
            base.setConfigValue("OutputDirectory", outputDirectory);

            string unsupportedLanguageDirectory = managerEnvironment + "unsupported\\";
            if (Directory.Exists(unsupportedLanguageDirectory) == false)
            {
                Directory.CreateDirectory(unsupportedLanguageDirectory);
            }
            base.setConfigValue("UnsupportedLanguageDirectory", unsupportedLanguageDirectory);
            button4.Enabled = true;
            this.Refresh();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show(this.resourceManager.GetString("information.autorunPS"), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                base.setConfigValue("AutoRunPS", "1");
                base.setConfigValue("PSPath", Helpers.FormHelper.getPokerStarsPath());
            }
            else
            {
                base.setConfigValue("AutoRunPS", "0");
            }
            HandManagerForm f = new ArchiveForm(this);
            f.Show();
            this.Hide();
        }
    }
}
