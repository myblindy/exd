using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using exd.World.Helpers;
using exd.World.ResourceCostHelper;

namespace exd.World.Resources
{
    public class ResourceGroundStack : Placeable
    {
        public ResourceCosts ResourceCosts;

        public ResourceGroundStack(WorldLocation location, ResourceCosts resources)
            : base(location, PlaceableRotation.Rotate0Degrees, null)
        {
            ResourceCosts = resources;
        }
    }
}
