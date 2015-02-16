using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using exd.World.AI;
using exd.World.Helpers;
using exd.World.ResourceCostHelper;
using exd.World.Resources;

namespace exd.World
{
    public static class GameWorld
    {
        public const int PartitionWidth = 200;
        public const int PartitionHeight = 200;

        /// <summary>
        /// Map size (in tiles)
        /// </summary>
        public static WorldDimension MapSize { get; private set; }

        /// <summary>
        /// Screen size (in tiles)
        /// </summary>
        public static WorldDimension ScreenSize { get; set; }

        /// <summary>
        /// Where the camera is centered
        /// </summary>
        public static WorldLocation CameraLocation { get; set; }

        /// <summary>
        /// The placeables in the world
        /// </summary>
        public static Placeables Placeables;

        /// <summary>
        /// The main storage location for the AI
        /// </summary>
        public static ActorCentralIntelligence ActorCentralIntelligence { get; set; }

        /// <summary>
        /// The random number generator
        /// </summary>
        public static Random Random;

        /// <summary>
        /// Initializes the game world
        /// </summary>
        public static void Initialize(int sizeX, int sizeY, int? rngseed = null)
        {
            ResourceProperties.Initialize();
            Placeables = new Placeables();
            ActorCentralIntelligence = new AI.ActorCentralIntelligence();
            Random = rngseed.HasValue ? new Random(rngseed.Value) : new Random();

            MapSize = new WorldDimension(sizeX, sizeY);
            ScreenSize = new WorldDimension();
            CameraLocation = new WorldLocation();
        }

        public static void Update(double delta)
        {
            Now += delta;
            Placeables.Update(delta);
        }

        /// <summary>
        /// Returns if any object is already in the way
        /// </summary>
        public static bool IsSolidInTheWay(WorldLocation location)
        {
            var placeables = Placeables.GetPlaceables(location);

            if (placeables == null)
                return false;
            else
                return placeables.Any();
        }

        public static bool IsLocationValid(WorldLocation location)
        {
            return location.X >= 0 && location.Y >= 0 && location.X <= MapSize.Width && location.Y <= MapSize.Height;
        }

        /// <summary>
        /// The game world internal time in ms (0 is the start of time)
        /// </summary>
        public static double Now { get; private set; }
    }
}
