using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryCube.Interfaces.Vision
{
    public interface IVisionService
    {
        /// <summary>
        /// Locates the pickup nozzle using vision and returns its 2D coordinates.
        /// </summary>
        Task<Position2D> LocatePickupNozzleAsync();
        Task<Position2D> LocateEjectionPointAsync();
    }

    public class Position2D
    {
        public double X { get; set; }
        public double Y { get; set; }
    }
}
