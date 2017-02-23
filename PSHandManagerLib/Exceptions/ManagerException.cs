using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.IO;

namespace PSHandManagerLib.Exceptions
{
    class ManagerException : Exception
    {
        /// <summary>
        /// Holds the localized errormessages
        /// IDs:
        /// 100 - 199 -> Manager
        /// 200 - 299 -> FileSystemWatcher + FileProcessor
        /// 300 - 399 -> HandProcessor
        /// </summary>
        public static Dictionary<int, String> messages;

        public static void initializeLocalizedErrorMessages()
        {
            using (FileStream fs = new FileStream(Manager.appPath + "\\Localizations\\Exceptions\\" + Manager.culture + ".json", FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using(StreamReader sr = new StreamReader(fs,new UTF8Encoding()))
                {
                    ManagerException.messages = JsonConvert.DeserializeObject<Dictionary<int, String>>(sr.ReadToEnd());
                }
            }
        }
        public static ManagerException createManagerException(int messageID, object[] variables, Exception innerException)
        {
            string completeMessage = String.Format(ManagerException.messages[messageID], variables);
            return new ManagerException(completeMessage, innerException);
        }

        private ManagerException(string completemessage, Exception innerException) : base(completemessage, innerException)
        {

        }
    }
}
