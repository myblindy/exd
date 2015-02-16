using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace exd.World.Helpers
{
    [DebuggerDisplay("{Width}x{Height}")]
    public struct WorldDimension
    {
        public long Width, Height;

        public WorldDimension(long w, long h) { Width = w; Height = h; }

        public override bool Equals(object obj)
        {
            if (obj is WorldDimension)
            {
                var loc = (WorldDimension)obj;
                return loc.Width == Width && loc.Height == Height;
            }
            else
                return false;
        }

        public static bool operator ==(WorldDimension a, WorldDimension b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(WorldDimension a, WorldDimension b)
        {
            return !a.Equals(b);
        }

        public override int GetHashCode()
        {
            return Width.GetHashCode() ^ Height.GetHashCode();
        }

        public static readonly WorldDimension OneByOne = new WorldDimension(0, 0);
    }
}
