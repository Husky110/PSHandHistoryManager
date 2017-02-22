using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSHandManagerLib.Tasks
{
    [Serializable()] 
    public struct HandTask
    {
        public string handSourceFilename;
        public string handNum;
        public string[] lines;
    }
}
