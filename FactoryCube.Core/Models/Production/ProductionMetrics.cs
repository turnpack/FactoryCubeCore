using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryCube.Core.Models.Production
{
    public class ProductionMetrics
    {
        public int TotalProcessed { get; set; }
        public int GoodCount { get; set; }
        public double UPH { get; set; }
        public TimeSpan AvgCycleTime { get; set; }
    }
}
