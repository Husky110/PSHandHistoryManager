using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Text;

namespace PSHandManagerLib.Localizations
{
    /// <summary>
    /// Detects the language of a hand within the first 20 lines...
    /// <seealso cref="HandLanguageDetector.detectHandLanguage(List{string})"/>
    /// </summary>
    class HandLanguageDetector
    {
        /// <summary>
        /// Holds one instance of the language-dictionary
        /// </summary>
        private static Dictionary<String, Dictionary<String, String>> languages = null;

        /// <summary>
        /// Returns a new Instance of the language-dictionary
        /// </summary>
        /// <returns></returns>
        private static Dictionary<String, Dictionary<String, String>> getDictionaryInstance()
        {
            if(HandLanguageDetector.languages == null)
            {
                using(FileStream fs = new FileStream(Manager.appPath + "\\Localizations\\LanguageDetector\\languages.json", FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    using (StreamReader sr = new StreamReader(fs, new UTF8Encoding()))
                    {
                        HandLanguageDetector.languages = JsonConvert.DeserializeObject<Dictionary<String, Dictionary<String, String>>>(sr.ReadToEnd());
                    }
                }
            }
            return new Dictionary<String, Dictionary<String, String>>(HandLanguageDetector.languages);
        }

        /// <summary>
        /// Used to initiate the languages only once.
        /// </summary>
        public static void initializeHandLanguageDetector()
        {
            HandLanguageDetector.getDictionaryInstance();
        }
        /// <summary>
        /// Detects the langauage of a hand
        /// </summary>
        /// <remarks>
        /// Okay, this one is a bit tricky...
        /// I had to find a way to detect the language of a hand.
        /// So I gathered Handhistories for every language available and analyzed them.
        /// Every third line of a hand looks kinda like this: "Seat x: player(y in chips)".
        /// So what I did is, that I created a dictionary on the word "Seat" in every language.
        /// Sometimes, 2 languages have the same word for seat - like Chinese and Taiwanese.
        /// In that case, I loop through the first 20 lines so find a stringpattern and recognize the language there.
        /// The only exception so far is Hungarian (Magyar) where the seat-line starts with a number. That's why I handle it seperatly...
        /// If PS ever does that to another language... well then I would have to think about a new system...
        /// </remarks>
        /// <param name="handLines">The lines of the hand</param>
        /// <param name="sourceFileName">The sourcefilepath - needed for exceptionhandling</param>
        /// <param name="HeroName">Name of the Heroplayer</param>
        /// <returns></returns>
        public string detectHandLanguage(List<String> handLines, string sourceFilePath, string HeroName)
        {
            Dictionary<string, Dictionary<string, string>> dictionary = HandLanguageDetector.getDictionaryInstance();
            string firstIndicatorLine = handLines[2];
            if (Char.IsNumber(firstIndicatorLine.Substring(0, 1).ToCharArray()[0]))
            {
                return "Magyar";
            }
            foreach(string key in dictionary.Keys)
            {
                if (firstIndicatorLine.StartsWith(key))
                {
                    if(dictionary[key].Keys.Count > 1)
                    {
                        Dictionary<String, String> subdict = dictionary[key];
                        foreach(string subkey in subdict.Keys)
                        {
                            string comparison = String.Format(subkey, HeroName);
                            for(int x = 2; x < 20; x++)
                            {
                                if (handLines[x].StartsWith(comparison))
                                {
                                    return subdict[subkey];
                                }
                            }
                        }
                    }
                    else
                    {
                        return dictionary[key]["1"];
                    }
                }
            }
            // if we arive here, than the language could not be detected... well... shit...
            throw PSHandManagerLib.Exceptions.ManagerException.createManagerException(400, new object[1] { sourceFilePath }, new NotImplementedException());
        }
    }
}
