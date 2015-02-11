using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace exd.World.Helpers
{
    [DebuggerDisplay("{X}, {Y}")]
    public struct WorldPartition
    {
        public long X, Y;

        public WorldPartition(long x, long y) { X = x; Y = y; }

        public override bool Equals(object obj)
        {
            if (obj is WorldPartition)
            {
                var loc = (WorldPartition)obj;
                return loc.X == X && loc.Y == Y;
            }
            else
                return false;
        }

        public static bool operator ==(WorldPartition a, WorldPartition b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(WorldPartition a, WorldPartition b)
        {
            return !a.Equals(b);
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }
    }
}
