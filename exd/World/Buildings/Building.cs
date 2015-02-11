using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using exd.World.Helpers;

namespace exd.World.Buildings
{
    public abstract class Building : Placeable
    {
        public Building(WorldLocation location, WorldDimension dimension, PlaceableRotation rotation, double? dob = null)
            : base(location, dimension, rotation, dob)
        {
        }

        public abstract string BlueprintName { get; }
    }
}
