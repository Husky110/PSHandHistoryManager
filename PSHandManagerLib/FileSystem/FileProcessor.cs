using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.Threading.Tasks;
using PSHandManagerLib.Tasks;

namespace PSHandManagerLib.FileSystem
{
    class FileProcessor
    {

        private string sourceFilePath = "";
        private string workingDirectory = "";
        public Task attachedTask; //used to remove the Task from the FileSystemWatcher
           
        public int detectedHands = 0; // to have a callback to the GUI in case that an old file is beeing imported.

        public FileProcessor(string filePath, string pathToWorkingDirectory)
        {
            this.sourceFilePath = filePath;
            this.workingDirectory = pathToWorkingDirectory;
        }

        public void processFile()
        {
            //this way is provided to process files which contain more than 1 hand

            List<String> fileLines = new List<string>();
            XmlSerializer serializer = new XmlSerializer(typeof(HandTask));
            using (FileStream fs = new FileStream(this.sourceFilePath, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    while (sr.Peek() > -1)
                    {
                        fileLines.Add(sr.ReadLine());
                    }
                }
            }
            File.Delete(this.sourceFilePath);

            int freespacecounter = 0;
            List<String> handLines = new List<string>();

            foreach (String line in fileLines)
            {
                if (line != "") // normal Line
                {
                    handLines.Add(line);
                }
                else // every PS-Hand ends with 3 empty lines... we've got one here...
                {
                    freespacecounter++;
                }
                if(freespacecounter == 3) //create a new HandTask out of the previous lines, since we've found 3 empty lines
                {
                    this.detectedHands++;
                    HandTask ht = new HandTask();
                    ht.handSourceFilename = Path.GetFileNameWithoutExtension(this.sourceFilePath);
                    //Handnumberdetection
                    string handnum = handLines[0].Split(':')[0];
                    char[] handnumArray = handnum.ToCharArray();
                    for (int x = handnumArray.Length - 1; x > 0; x--)
                    {
                        if (Char.IsNumber(handnumArray[x]) == false && x > 0)
                        {
                            handnum = handnum.Substring(x + 1, handnum.Length - x - 1);
                            break;
                        }
                    }
                    ht.handNum = handnum;
                    ht.lines = new string[handLines.Count];
                    handLines.CopyTo(ht.lines);
                    using (FileStream fs = new FileStream(this.workingDirectory + handnum + ".xml", FileMode.CreateNew, FileAccess.Write, FileShare.None))
                    {
                        serializer.Serialize(fs, ht);
                    }
                    handLines = new List<string>();
                    freespacecounter = 0;
                }
            }
            bool y = true; //just to be there... I ignore the out value actually...
            FileSystemWatcher.files.TryRemove(this.sourceFilePath, out y);
            HandProcessor.runningTasks.TryRemove(this.attachedTask, out y);
        }
    }
}
