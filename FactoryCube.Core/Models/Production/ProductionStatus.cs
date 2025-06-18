using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryCube.Core.Models.Production
{
    public class ProductionStatus
    {
        public bool IsRunning { get; set; }
        public bool IsPaused { get; set; }
        public string CurrentStep { get; set; }
    }
}
