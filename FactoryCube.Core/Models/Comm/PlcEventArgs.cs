using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryCube.Core.Models.Comm
{
    public class PlcEventArgs : EventArgs
    {
        public string TagName { get; set; }
        public object Value { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
