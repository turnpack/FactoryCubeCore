using FactoryCube.UI.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryCube.UI.ViewModels
{
    public abstract class RoiItem : ViewModelBase
    {
        public Guid Id { get; } = Guid.NewGuid();
        public string Name { get; set; }
        public abstract RoiShape Shape { get; }
    }

    public class RectangleRoiItem : RoiItem
    {
        public override RoiShape Shape => RoiShape.Rectangle;
        public double CenterX { get; set; }
        public double CenterY { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
    }
}
