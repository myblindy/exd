using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using exd.World.AI;
using exd.World.Helpers;
using exd.World.ResourceCostHelper;

namespace exd.World.Buildings
{
    /// <summary>
    /// Resource storage
    /// </summary>
    public class Storage : Building
    {
        private ResourceCosts InternalResourceCosts = new ResourceCosts { { ResourceType.Wood, 140 } };

        public Storage(WorldLocation location, WorldDimension dimension, double? dob = null)
            : base(location, dimension, PlaceableRotation.Rotate0Degrees, dob)
        {
            // can't be rotated, has a variable dimension
        }

        public override string BuildingName { get { return "Storage"; } }

        public override double BuiltBuildupRequired { get { return 250; } }

        public override ResourceCosts ResourceCosts { get { return InternalResourceCosts; } }

        public override double GetDropDuration(Actor actor) { return 20; }

    }
}
