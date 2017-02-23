using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.IO;

namespace PSHandManagerLib.Exceptions
{
    /// <summary>
    /// Is used for all Exceptions that occur
    /// </summary>
    class ManagerException : Exception
    {
        /// <summary>
        /// Holds the localized errormessages
        /// <remarks>
        /// IDs:
        /// 100 - 199 -> Manager
        /// 200 - 299 -> FileSystemWatcher + FileProcessor
        /// 300 - 399 -> HandProcessor
        /// 400 - 499 -> Language
        /// </remarks>
        /// </summary>
        public static Dictionary<int, String> messages;

        /// <summary>
        /// Loads the localizations of errormessages based on the culture set in App.config
        /// </summary>
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
        /// <summary>
        /// Used to create a new Exception.
        /// Since the message has to be loaded within the constructor, this method uses a private constructor to load the baseconstructor, which enables a fully qualified exception;
        /// </summary>
        /// <param name="messageID">The id of the occured error</param>
        /// <param name="variables">Obejctarray which holds the used variables for String.Format on the exceptionmessage</param>
        /// <param name="innerException">The catched baseexception for later use</param>
        /// <returns></returns>
        public static ManagerException createManagerException(int messageID, object[] variables, Exception innerException)
        {
            string completeMessage = String.Format(ManagerException.messages[messageID], variables);
            return new ManagerException(completeMessage, innerException);
        }

        /// <summary>
        /// Private constructor to load the base constructor with pre-set values
        /// </summary>
        /// <param name="completemessage">The complete localized and formated errormessage</param>
        /// <param name="innerException">The catched baseexception for later use</param>
        private ManagerException(string completemessage, Exception innerException) : base(completemessage, innerException)
        {

        }
    }
}
