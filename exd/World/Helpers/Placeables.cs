using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace exd.World.Helpers
{
    /// <summary>
    /// Contains and manages every resource in the world
    /// </summary>
    public class Placeables : IUpdateable
    {
        /// <summary>
        /// The list of every resource in the world (partitioned)
        /// </summary>
        private Dictionary<WorldPartition, List<Placeable>> PartitionedPlaceablesCollection = new Dictionary<WorldPartition, List<Placeable>>();

        /// <summary>
        /// Get the placeables contained in a rectangle of partitions
        /// </summary>
        public IEnumerable<Placeable> GetPlaceables(Rectangle locations)
        {
            for (int x = locations.Left; x <= locations.Right; ++x)
                for (int y = locations.Top; y <= locations.Bottom; ++y)
                {
                    List<Placeable> placeables = null;
                    if (PartitionedPlaceablesCollection.TryGetValue(new WorldPartition(x, y), out placeables))
                        foreach (var placeable in placeables)
                            if (!placeable.Dead)
                                yield return placeable;
                }
        }

        /// <summary>
        /// Get the placeables contained in a list of partitions
        /// </summary>
        public IEnumerable<Placeable> GetPlaceables(IEnumerable<WorldPartition> locations)
        {
            foreach (var location in locations)
            {
                List<Placeable> placeables = null;
                if (PartitionedPlaceablesCollection.TryGetValue(location, out placeables))
                    foreach (var placeable in placeables)
                        if (!placeable.Dead)
                            yield return placeable;
            }
        }

        /// <summary>
        /// Get the placeables at a certain position
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public IEnumerable<Placeable> GetPlaceables(WorldLocation location)
        {
            // get the partition
            var partpos = location.ToWorldPartition();
            List<Placeable> placeables = null;
            if (!PartitionedPlaceablesCollection.TryGetValue(partpos, out placeables))
                return new Placeable[0];
            else
                return placeables.Where(t => t.Intersects(location));
        }

        /// <summary>
        /// Add a new resource to the world
        /// </summary>
        /// <param name="location">The resource location</param>
        /// <param name="dob">Date it's added (null for this moment)</param>
        /// <param name="lifetime">How long it will live (null to generate it randomly)</param>
        /// <param name="dontcheck">Whether or not to check if the resource already exists (for performance reasons)</param>
        /// <returns></returns>
        public bool Add(Placeable placeable, bool dontcheck = false)
        {
            if (!dontcheck && GameWorld.IsSolidInTheWay(placeable.Location))
                return false;

            List<Placeable> placeables;
            var partpos = placeable.Location.ToWorldPartition();
            if (!PartitionedPlaceablesCollection.TryGetValue(partpos, out placeables))
                PartitionedPlaceablesCollection.Add(partpos, placeables = new List<Placeable>());

            placeables.Add(placeable);

            return true;
        }

        public bool Add(IEnumerable<Placeable> placeables, bool dontcheck = false)
        {
            var res = true;
            foreach (var placeable in placeables)
                res &= Add(placeable, dontcheck);

            return res;
        }

        /// <summary>
        /// Removes a resource from the world
        /// </summary>
        public bool Remove(WorldLocation location)
        {
            List<Placeable> placeables;
            var partpos = location.ToWorldPartition();
            if (!PartitionedPlaceablesCollection.TryGetValue(partpos, out placeables))
                return false;
            else
            {
                placeables.RemoveAll(t => t.Location == location);
                return true;
            }
        }

        /// <summary>
        /// Update all placeables
        /// </summary>
        public void Update(double delta)
        {
            // update the current placeables
            foreach (var kvp in PartitionedPlaceablesCollection)
                foreach (var placeable in kvp.Value.OfType<IUpdateable>())
                    placeable.Update(delta);

            // remove any dead placeables
            foreach (var kvp in PartitionedPlaceablesCollection)
                kvp.Value.RemoveAll(t => t.Dead);
        }
    }
}
