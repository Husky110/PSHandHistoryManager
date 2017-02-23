using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSHandManagerLib.Tasks
{
    /// <summary>
    /// Strucutre to store hands which were found in PokerStars-files and make them serializeable.
    /// </summary>
    [Serializable()] 
    public struct HandTask
    {
        /// <summary>
        /// Filename of the PokerStars-file without extension
        /// </summary>
        public string handSourceFilename;

        /// <summary>
        /// PokerStarsHand-ID
        /// PokerStars uses a x64 unsigned int for the ID.
        /// In order to run the Manager on a x86-System this ID is stored as a string.
        /// </summary>
        public string handNum;

        /// <summary>
        /// The lines to the hand from the PokerStars-file.
        /// </summary>
        public string[] lines;
    }
}
