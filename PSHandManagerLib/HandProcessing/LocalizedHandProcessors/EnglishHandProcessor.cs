using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PSHandManagerLib.Tasks;

namespace PSHandManagerLib.HandProcessing.LocalizedHandProcessors
{
    class EnglishHandProcessor : HandProcessor
    {
        public EnglishHandProcessor(HandTask ht) : base(ht){}

        public override void processHandTask()
        {

        }

        protected override List<string> detectPlayers()
        {
            throw new NotImplementedException();
        }
    }
}
