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
    public abstract class AbstractGroundResource : Placeable
    {
        public double? DateOfDeath { get; set; }
        private bool DeadOverride = false;
        public override bool Dead
        {
            get
            {
                return DeadOverride || !DateOfDeath.HasValue || GameWorld.Now >= DateOfDeath;
            }
            set
            {
                DeadOverride = value;
            }
        }

        public AbstractGroundResource(WorldLocation location, WorldDimension dim, PlaceableRotation rot, double? dob, double? lifetime)
            : base(location, dim, rot, dob)
        {
            DateOfDeath = lifetime.HasValue ? lifetime + DateOfBirth : null;
        }

        public abstract double GetGatherDuration(Actor actor);
        public abstract ResourceCosts GetRemainingResources();
        public abstract void GatherFinished(Actor actor);
    }
}
