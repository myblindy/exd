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
            PromisedResourceCosts = new Dictionary<PromiseToken, ResourceCosts>();
        }

        public abstract string BuildingName { get; }
        public abstract ResourceCosts ResourceCosts { get; }
        protected Dictionary<PromiseToken, ResourceCosts> PromisedResourceCosts { get; private set; }

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

        public PromiseToken Promise(ResourceCosts res)
        {
            PromiseToken token = new PromiseToken(t => PromiseFinished(t));
            PromisedResourceCosts.Add(token, res);

            return token;
        }

        public void PromiseFinished(PromiseToken token)
        {
            PromisedResourceCosts.Remove(token);
        }

        public bool PromiseCovers(ResourceCosts resourceCosts)
        {
            foreach (var kvp in resourceCosts)
                if (kvp.Value + PromisedResourceCosts.Sum(kvp1 => kvp1.Value[kvp.Key]) > 0)
                    return false;

            return true;
        }
    }
}
