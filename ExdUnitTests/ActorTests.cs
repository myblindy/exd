using System;
using System.Drawing;
using exd.World;
using exd.World.AI;
using exd.World.Helpers;
using exd.World.Resources;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace ExdUnitTests
{
    [TestClass]
    public class ActorTests
    {
        [TestMethod]
        public void CuttingTrees()
        {
            GameWorld.Initialize(100, 100, 50);

            // plant trees
            var forest = new[] 
            { 
                new Tree(new WorldLocation(3, 3)),
                new Tree(new WorldLocation(3, 4)),
                new Tree(new WorldLocation(3, 5))
            };
            Assert.IsTrue(GameWorld.Placeables.Add(forest));
            Assert.IsTrue(GameWorld.Placeables.GetPlaceables(new WorldLocation(3, 3)).OfType<Tree>().Count() == 1);
            Assert.IsTrue(GameWorld.Placeables.GetPlaceables(new WorldLocation(3, 4)).OfType<Tree>().Count() == 1);
            Assert.IsTrue(GameWorld.Placeables.GetPlaceables(new WorldLocation(3, 5)).OfType<Tree>().Count() == 1);
            Assert.IsFalse(forest.Any(t => t.Dead));

            // create an actor
            var actor = new Actor(new WorldLocation(0, 0));
            Assert.IsTrue(GameWorld.Placeables.Add(actor));

            // add tasks to cut the trees
            GameWorld.ActorCentralIntelligence.AddGatherTask(forest);

            // simulate the game loop
            for (int i = 0; i < 10000; ++i)
                GameWorld.Update(10);

            Assert.IsTrue(GameWorld.Placeables.GetPlaceables(new WorldLocation(3, 3)).OfType<Tree>().Count() == 0);
            Assert.IsTrue(GameWorld.Placeables.GetPlaceables(new WorldLocation(3, 4)).OfType<Tree>().Count() == 0);
            Assert.IsTrue(GameWorld.Placeables.GetPlaceables(new WorldLocation(3, 5)).OfType<Tree>().Count() == 0);
            Assert.IsFalse(forest.Any(t => !t.Dead));
            Assert.IsTrue(GameWorld.Placeables.GetPlaceables(new WorldLocation(3, 5)).OfType<ResourceGroundStack>().Count() == 1);
        }
    }
}
