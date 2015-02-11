using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace exd.World.ResourceCostHelper
{
    public enum ResourceType
    {
        [ResourceWeight(20)]
        Wood
    }

    public class ResourceWeightAttribute : Attribute
    {
        public double Weight { get; set; }

        public ResourceWeightAttribute(double weight) { Weight = weight; }
    }
}
