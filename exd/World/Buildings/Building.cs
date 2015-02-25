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
    public abstract class Building : Placeable
    {
        public Building(WorldLocation location, WorldDimension dimension, PlaceableRotation rotation, double? dob = null)
            : base(location, dimension, rotation, dob)
        {
            PromisedResourceCosts = new ResourceCosts();
        }

        public abstract string BuildingName { get; }
        public abstract ResourceCosts ResourceCosts { get; }
        public ResourceCosts PromisedResourceCosts { get; private set; }

        public double BuiltBuildup { get; set; }
        public abstract double BuiltBuildupRequired { get; }
        public bool Built { get { return BuiltBuildup >= BuiltBuildupRequired; } }

        public abstract double GetDropDuration(Actor actor);
        public virtual void DropResourcesFinished(Actor actor)
        {
            if (!Built)
            {
                var resourcetypes = actor.ResourcesCarried.Select(r => r.Key).ToArray();

                foreach (var resourcetype in resourcetypes)
                {
                    var costqty = ResourceCosts[resourcetype];
                    var actqty = actor.ResourcesCarried[resourcetype];

                    if (costqty > 0 && actqty < 0)
                    {
                        var qty = Math.Min(-actqty, costqty);
                        actor.ResourcesCarried.Add(resourcetype, qty);
                        ResourceCosts.Add(resourcetype, -qty);
                    }
                }
            }
        }
    }
}
