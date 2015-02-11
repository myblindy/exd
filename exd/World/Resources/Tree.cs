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
    /// <summary>
    /// A single tree
    /// </summary>
    public class Tree : AbstractGroundResource
    {
        private static double GetRandomTreeLifetime()
        {
            const double baselifetime = 40 * 365;
            const double rnglifetime = 40 * 365;

            return baselifetime + GameWorld.Random.NextDouble() * rnglifetime;
        }

        private static ResourceCosts TreeResourceCost = new ResourceCosts { { ResourceType.Wood, -20 } };

        public Tree(WorldLocation location, double? dob = null, double? dod = null)
            : base(location, WorldDimension.OneByOne, PlaceableRotation.Rotate0Degrees, dob, dod ?? GetRandomTreeLifetime())
        {
        }

        public override double GetGatherDuration(Actor actor)
        {
            return actor.GatherSpeed / -TreeResourceCost[ResourceType.Wood];
        }

        public override ResourceCosts GetRemainingResources()
        {
            return TreeResourceCost;
        }

        public override void GatherFinished(Actor actor)
        {
            actor.AddCarry(GetRemainingResources());
            Dead = true;
        }
    }
}
