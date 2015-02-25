using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using exd.World.AI;
using exd.World.Helpers;
using exd.World.ResourceCostHelper;

namespace exd.World.Resources
{
    public class ResourceGroundStack : AbstractGroundResource
    {
        public ResourceCosts ResourceCosts;

        public ResourceGroundStack(WorldLocation location, ResourceCosts resources)
            : base(location, WorldDimension.OneByOne, PlaceableRotation.Rotate0Degrees, null, null)
        {
            ResourceCosts = resources;
        }

        public override double GetGatherDuration(Actor actor)
        {
            return actor.GatherSpeed;
        }

        public override ResourceCosts GetRemainingResources()
        {
            return ResourceCosts;
        }

        public override void GatherFinished(Actor actor)
        {
            actor.AddCarry(ResourceCosts);
            Dead = true;
        }
    }
}
