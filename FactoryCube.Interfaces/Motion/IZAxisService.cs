using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryCube.Interfaces.Motion
{
    public interface IZAxisService
    {
        /// <summary>
        /// Performs Z-axis validation (e.g. touch-off, safe height check, etc.)
        /// </summary>
        Task ValidateZHeightAsync();
    }
}
