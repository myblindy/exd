using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace exd.World.Helpers
{
    /// <summary>
    /// A world partition (usually built from a world location)
    /// </summary>
    [DebuggerDisplay("{X},{Y} {Width}x{Height}")]
    public struct WorldPartitionRectangle
    {
        public long X, Y, Width, Height;

        public WorldPartitionRectangle(long x, long y, long w, long h) { X = x; Y = y; Width = w; Height = h; }

        public override bool Equals(object rect)
        {
            if (rect is WorldPartitionRectangle)
            {
                var loc = (WorldPartitionRectangle)rect;
                return loc.X == X && loc.Y == Y && loc.Width == Width && loc.Height == Height;
            }
            else
                return false;
        }

        public static bool operator ==(WorldPartitionRectangle a, WorldPartitionRectangle b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(WorldPartitionRectangle a, WorldPartitionRectangle b)
        {
            return !a.Equals(b);
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode() ^ Width.GetHashCode() ^ Height.GetHashCode();
        }
    }
}
