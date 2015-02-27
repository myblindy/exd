using System;
using exd.World;
using exd.World.AI;
using exd.World.Helpers;
using exd.World.Resources;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using exd.World.ResourceCostHelper;
using exd.World.Buildings;

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
                new Tree(new WorldLocation(3, 4)),
                new Tree(new WorldLocation(3, 3)),
                new Tree(new WorldLocation(3, 5))
            };
            Assert.IsTrue(GameWorld.Placeables.Add(forest));
            Assert.IsTrue(GameWorld.Placeables.GetPlaceables(new WorldLocation(3, 3)).OfType<Tree>().Count() == 1);
            Assert.IsTrue(GameWorld.Placeables.GetPlaceables(new WorldLocation(3, 4)).OfType<Tree>().Count() == 1);
            Assert.IsTrue(GameWorld.Placeables.GetPlaceables(new WorldLocation(3, 5)).OfType<Tree>().Count() == 1);
            Assert.IsFalse(forest.Any(t => t.Dead));

            // create an actor
            var actor = new Actor(new WorldLocation(0, 0)) { Name = "Serge" };
            Assert.IsTrue(GameWorld.Placeables.Add(actor));

            // add tasks to cut the trees
            GameWorld.ActorCentralIntelligence.AddGatherTask(forest);

            // simulate the game loop
            for (int i = 0; i < 10000; ++i)
                GameWorld.Update(10);

            // and check that all trees are cut at the end
            Assert.IsTrue(GameWorld.Placeables.GetPlaceables(new WorldLocation(3, 3)).OfType<Tree>().Count() == 0);
            Assert.IsTrue(GameWorld.Placeables.GetPlaceables(new WorldLocation(3, 4)).OfType<Tree>().Count() == 0);
            Assert.IsTrue(GameWorld.Placeables.GetPlaceables(new WorldLocation(3, 5)).OfType<Tree>().Count() == 0);
            Assert.IsFalse(forest.Any(t => !t.Dead));
            Assert.IsTrue(GameWorld.Placeables.GetPlaceables(new WorldLocation(3, 5)).OfType<ResourceGroundStack>().Count() == 1);
        }

        private void BuildingWithStackedResources(int nactors)
        {
            GameWorld.Initialize(100, 100, 50);

            var actorlocations = new[] { new WorldLocation(5, 5), new WorldLocation(6, 5), new WorldLocation(7, 5) };
            var actornames = new[] { "Jeff", "Marla", "Scooby" };

            var actors = Enumerable.Range(0, nactors)
                .Select(i => new Actor(actorlocations[i]) { Name = actornames[i] })
                .ToArray();

            // spawn the actors
            foreach (var actor in actors)
                Assert.IsTrue(GameWorld.Placeables.Add(actor));

            // spawn some wood stacks around them
            Assert.IsTrue(GameWorld.Placeables.Add(new ResourceGroundStack(new WorldLocation(2, 2),
                new ResourceCosts { { ResourceType.Wood, -50 } })));
            Assert.IsTrue(GameWorld.Placeables.Add(new ResourceGroundStack(new WorldLocation(2, 5),
                new ResourceCosts { { ResourceType.Wood, -40 } })));
            Assert.IsTrue(GameWorld.Placeables.Add(new ResourceGroundStack(new WorldLocation(3, 3),
                new ResourceCosts { { ResourceType.Wood, -90 } })));

            // place the stockpile blueprint
            var storage = new Storage(new WorldLocation(5, 10), new WorldDimension(5, 5));
            Assert.IsTrue(GameWorld.Placeables.Add(storage));

            // set the goal
            GameWorld.ActorCentralIntelligence.AddBuildTask(storage);

            // game loop
            for (int i = 0; i < 100000; ++i)
                GameWorld.Update(30);

            // and check the end state
            Assert.IsTrue(storage.Built);
        }

        [TestMethod]
        public void BuildingWithStackedResources1Actor()
        {
            BuildingWithStackedResources(1);
        }

        [TestMethod]
        public void BuildingWithStackedResources2Actors()
        {
            BuildingWithStackedResources(2);
        }

        [TestMethod]
        public void BuildingWithStackedResources3Actors()
        {
            BuildingWithStackedResources(3);
        }
    }
}
