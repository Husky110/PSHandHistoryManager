using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSHandManagerLib.HandProcessing
{
    /// <summary>
    /// Interface to handle all possible languages for hands the same way
    /// </summary>
    public interface IHandProcessor
    {
        Task attachedTask { get; set; }
        void processHandTask();
    }
}
