using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace exd.World.Helpers
{
    /// <summary>
    /// A rectangle of world locations
    /// </summary>
    [DebuggerDisplay("{X},{Y} {Width}x{Height}")]
    public struct WorldRectangle
    {
        public long X, Y, Width, Height;

        public WorldRectangle(long x, long y, long w, long h) { X = x; Y = y; Width = w; Height = h; }

        public override bool Equals(object rect)
        {
            if (rect is WorldRectangle)
            {
                var loc = (WorldRectangle)rect;
                return loc.X == X && loc.Y == Y && loc.Height == Height && loc.Width == Width;
            }
            else
                return false;
        }

        public static bool operator ==(WorldRectangle a, WorldRectangle b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(WorldRectangle a, WorldRectangle b)
        {
            return !a.Equals(b);
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode() ^ Width.GetHashCode() & Height.GetHashCode();
        }

        public override string ToString()
        {
            return X + "," + Y + "-" + (X + Width) + "," + (Y + Height);
        }

        public WorldRectangle Offset(long x, long y)
        {
            return new WorldRectangle(X + x, Y + y, Width, Height);
        }
    }
}
