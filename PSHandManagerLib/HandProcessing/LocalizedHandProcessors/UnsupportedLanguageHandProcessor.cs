using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using PSHandManagerLib.Tasks;
using System.IO;

namespace PSHandManagerLib.HandProcessing.LocalizedHandProcessors
{
    /// <summary>
    /// A class which processes a hand in a language we don't support yet...
    /// </summary>
    class UnsupportedLanguageHandProcessor : HandProcessor
    {
        /// <summary>
        /// Holds the path to the directory which saves all hands with an unsupported language
        /// </summary>
        protected string unsupportedLanguageDirectory = "";

        public UnsupportedLanguageHandProcessor(HandTask ht) : base(ht)
        {
            this.unsupportedLanguageDirectory = ConfigurationManager.AppSettings["UnsupportedLanguageDirectory"];
        }

        /// <summary>
        /// This has to be empty and never be called!
        /// Had to be defined, since the baseclass requires it but is actually unused code...
        /// </summary>
        /// <returns></returns>
        protected override Dictionary<String, int> detectPlayers()
        {
            return null;
        }

        /// <summary>
        /// Just takes the HandTask and writes the file unprocessed.
        /// </summary>
        public override void processHandTask()
        {
            this.WriteHandFile(this.unsupportedLanguageDirectory, handTask.lines);
            if (this.archiveUnprocessedHands)
            {
                string filePath = this.createDirectoriesForArchiveAndStorage(this.archiveDirectory) + this.outputFilename;
                this.WriteHandFile(filePath, this.handTask.lines);
            }

            if (this.storeProcessedHands)
            {
                string filePath = this.createDirectoriesForArchiveAndStorage(this.storageDirectory) + this.outputFilename;
                this.WriteHandFile(filePath, handTask.lines);
            }
            this.finishTask(true);
        }
    }
}
