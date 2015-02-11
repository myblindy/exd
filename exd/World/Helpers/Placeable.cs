using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace exd.World.Helpers
{
    public enum PlaceableRotation
    {
        Rotate0Degrees,
        Rotate90Degrees,
        Rotate180Degrees,
        Rotate270Degrees
    }

    /// <summary>
    /// Base class that describes a placeable
    /// </summary>
    public abstract class Placeable
    {
        public WorldLocation Location { get; protected set; }
        public WorldDimension Dimension { get; protected set; }
        public PlaceableRotation Rotation { get; protected set; }
        public double DateOfBirth { get; protected set; }

        private bool HelperIntersectsInRange(long value, long min, long max)
        {
            return (value >= min) && (value <= max);
        }

        public bool Intersects(Placeable obj)
        {
            var rotate = Rotation == PlaceableRotation.Rotate90Degrees || Rotation == PlaceableRotation.Rotate270Degrees;
            var ax = Location.X;
            var ay = Location.Y;
            var aw = rotate ? Dimension.Height : Dimension.Width;
            var ah = rotate ? Dimension.Width : Dimension.Height;

            rotate = obj.Rotation == PlaceableRotation.Rotate90Degrees || obj.Rotation == PlaceableRotation.Rotate270Degrees;
            var bx = obj.Location.X;
            var by = obj.Location.Y;
            var bw = rotate ? obj.Dimension.Height : obj.Dimension.Width;
            var bh = rotate ? obj.Dimension.Width : obj.Dimension.Height;

            bool xOverlap = HelperIntersectsInRange(ax, bx, bx + bw) ||
                            HelperIntersectsInRange(bx, ax, ax + aw);

            bool yOverlap = HelperIntersectsInRange(ay, by, by + bh) ||
                            HelperIntersectsInRange(by, ay, ay + ah);

            return xOverlap && yOverlap;
        }

        public bool Intersects(WorldLocation location)
        {
            var rotate = Rotation == PlaceableRotation.Rotate90Degrees || Rotation == PlaceableRotation.Rotate270Degrees;
            var ax = Location.X;
            var ay = Location.Y;
            var aw = rotate ? Dimension.Height : Dimension.Width;
            var ah = rotate ? Dimension.Width : Dimension.Height;

            return HelperIntersectsInRange(location.X, ax, ax + aw) &&
                HelperIntersectsInRange(location.Y, ay, ay + ah);
        }

        public virtual bool Dead { get { return false; } set { } }

        public virtual bool Passable { get { return true; } }

        public Placeable(WorldLocation location, WorldDimension dimension, PlaceableRotation rotation, double? dob)
        {
            Location = location;
            Dimension = dimension;
            Rotation = rotation;
            DateOfBirth = dob ?? GameWorld.Now;
        }

        public Placeable(WorldLocation location, PlaceableRotation rotation, double? dob)
            : this(location, WorldDimension.OneByOne, rotation, dob)
        {
        }
    }
}
