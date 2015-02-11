using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using exd.World.Helpers;

namespace exd.World.Buildings
{
    /// <summary>
    /// Resource storage
    /// </summary>
    public class Storage : Building
    {
        public Storage(WorldLocation location, WorldDimension dimension, double? dob = null)
            : base(location, dimension, PlaceableRotation.Rotate0Degrees, dob)
        {
            // can't be rotated, has a variable dimension
        }

        public override string BlueprintName
        {
            get { return "Storage"; }
        }
    }
}
