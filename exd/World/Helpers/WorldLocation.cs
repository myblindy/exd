using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace exd.World.Helpers
{
    [DebuggerDisplay("{X}, {Y}")]
    public struct WorldLocation
    {
        public long X, Y;

        public WorldLocation(long x, long y) { X = x; Y = y; }

        public override bool Equals(object obj)
        {
            if (obj is WorldLocation)
            {
                var loc = (WorldLocation)obj;
                return loc.X == X && loc.Y == Y;
            }
            else
                return false;
        }

        public static bool operator ==(WorldLocation a, WorldLocation b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(WorldLocation a, WorldLocation b)
        {
            return !a.Equals(b);
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }

        public WorldPartition ToWorldPartition()
        {
            return new WorldPartition(X / GameWorld.PartitionWidth, Y / GameWorld.PartitionHeight);
        }

        public override string ToString()
        {
            return X + "," + Y;
        }

        public WorldLocation Offset(long x, long y)
        {
            return new WorldLocation(X + x, Y + y);
        }

        public static double Distance(WorldLocation l1, WorldLocation l2)
        {
            return Math.Sqrt((l1.X - l2.X) * (l1.X - l2.X) + (l1.Y - l2.Y) * (l1.Y - l2.Y));
        }
    }
}
