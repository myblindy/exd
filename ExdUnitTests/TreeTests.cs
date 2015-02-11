using System;
using System.Drawing;
using exd.World;
using exd.World.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using exd.World.Resources;

namespace ExdUnitTests
{
    [TestClass]
    public class TreeTests
    {
        [TestMethod]
        public void BasicAddingTrees()
        {
            GameWorld.Initialize(1000, 1000, 1);

            Assert.IsTrue(GameWorld.Placeables.Add(new Tree(new WorldLocation(0, 0))), "Adding tree at 0,0");
            Assert.IsTrue(GameWorld.Placeables.Add(new Tree(new WorldLocation(100, 100))), "Adding tree at 100,100");
            Assert.IsTrue(GameWorld.Placeables.Add(new Tree(new WorldLocation(420, 420))), "Adding tree at 420,420");
            Assert.IsFalse(GameWorld.Placeables.Add(new Tree(new WorldLocation(100, 100))), "Adding tree at 100,100");
            Assert.IsTrue(GameWorld.Placeables.Add(new Tree(new WorldLocation(100, 100)), true), "Adding tree at 100,100");

            Assert.IsTrue(GameWorld.Placeables.GetPlaceables(new WorldLocation(0, 0)).Count() == 1, "Tree exists at 0,0");
            Assert.IsTrue(GameWorld.Placeables.GetPlaceables(new WorldLocation(100, 100)).Count() == 2, "Tree exists at 100,100");
            Assert.IsTrue(GameWorld.Placeables.GetPlaceables(new WorldLocation(420, 420)).Count() == 1, "Tree exists at 420,420");
            Assert.IsTrue(GameWorld.Placeables.GetPlaceables(new WorldLocation(1, 1)).Count() == 0, "Tree exists at 1,1");
            Assert.IsTrue(GameWorld.Placeables.GetPlaceables(new WorldLocation(420, 1)).Count() == 0, "Tree exists at 420,1");

            var trees = GameWorld.Placeables.GetPlaceables(new Rectangle(0, 0, 0, 0)).ToArray();
            Assert.IsTrue(trees.Count() == 3, "Partition 0,0 contains 3 trees");
            Assert.IsTrue(trees.Where(t => t.Location == new WorldLocation(0, 0)).Count() == 1, "Partition 0,0 contains one tree at 0,0");
            Assert.IsTrue(trees.Where(t => t.Location == new WorldLocation(100, 100)).Count() == 2, "Partition 0,0 contains two trees at 100,100");
            Assert.IsFalse(trees.Where(t => t.Location == new WorldLocation(420, 420)).Count() == 1, "Partition 0,0 contains one tree at 420,420");

            trees = GameWorld.Placeables.GetPlaceables(new Rectangle(0, 0, 1, 1)).ToArray();
            Assert.IsTrue(trees.Count() == 3, "Partitions (0,0)-(1,1) contain 3 trees");

            trees = GameWorld.Placeables.GetPlaceables(new Rectangle(0, 0, 2, 2)).ToArray();
            Assert.IsTrue(trees.Count() == 4, "Partitions (0,0)-(2,2) contain 4 trees");
            Assert.IsTrue(trees.Where(t => t.Location == new WorldLocation(420, 420)).Count() == 1, "Partitions (0,0)-(2,2) contain one tree at 420,420");

            trees = GameWorld.Placeables.GetPlaceables(new[] { new WorldPartition(0, 0), new WorldPartition(1, 1) }).ToArray();
            Assert.IsTrue(trees.Count() == 3, "Partitions (0,0) and (1,1) contain 3 trees");

            trees = GameWorld.Placeables.GetPlaceables(new[] { new WorldPartition(0, 0), new WorldPartition(2, 2) }).ToArray();
            Assert.IsTrue(trees.Count() == 4, "Partitions (0,0) and (2,2) contain 4 trees");
        }

        [TestMethod]
        public void BasicTreeAging()
        {
            GameWorld.Initialize(1000, 1000, 1);

            Assert.IsTrue(GameWorld.Placeables.Add(new Tree(new WorldLocation(0, 0), null, 40)), "Adding tree at 0,0");
            Assert.IsTrue(GameWorld.Placeables.Add(new Tree(new WorldLocation(5, 5), null, 50)), "Adding tree at 5,5");
            Assert.IsTrue(GameWorld.Placeables.Add(new Tree(new WorldLocation(6, 6), null, 60)), "Adding tree at 6,6");
            Assert.IsTrue(GameWorld.Placeables.Add(new Tree(new WorldLocation(7, 7), null, 70)), "Adding tree at 7,7");

            Assert.IsNotNull(GameWorld.Placeables.GetPlaceables(new WorldLocation(0, 0)));
            GameWorld.Update(40);
            Assert.IsTrue(GameWorld.Placeables.GetPlaceables(new WorldLocation(0, 0)).Count() == 0);
            Assert.IsTrue(GameWorld.Placeables.GetPlaceables(new WorldLocation(5, 5)).Count() == 1);
            Assert.IsTrue(GameWorld.Placeables.GetPlaceables(new WorldLocation(6, 6)).Count() == 1);
            Assert.IsTrue(GameWorld.Placeables.GetPlaceables(new WorldLocation(7, 7)).Count() == 1);

            GameWorld.Update(10);
            Assert.IsTrue(GameWorld.Placeables.GetPlaceables(new WorldLocation(0, 0)).Count() == 0);
            Assert.IsTrue(GameWorld.Placeables.GetPlaceables(new WorldLocation(5, 5)).Count() == 0);
            Assert.IsTrue(GameWorld.Placeables.GetPlaceables(new WorldLocation(6, 6)).Count() == 1);
            Assert.IsTrue(GameWorld.Placeables.GetPlaceables(new WorldLocation(7, 7)).Count() == 1);

            GameWorld.Update(10);
            Assert.IsTrue(GameWorld.Placeables.GetPlaceables(new WorldLocation(0, 0)).Count() == 0);
            Assert.IsTrue(GameWorld.Placeables.GetPlaceables(new WorldLocation(5, 5)).Count() == 0);
            Assert.IsTrue(GameWorld.Placeables.GetPlaceables(new WorldLocation(6, 6)).Count() == 0);
            Assert.IsTrue(GameWorld.Placeables.GetPlaceables(new WorldLocation(7, 7)).Count() == 1);

            GameWorld.Update(10);
            Assert.IsTrue(GameWorld.Placeables.GetPlaceables(new WorldLocation(0, 0)).Count() == 0);
            Assert.IsTrue(GameWorld.Placeables.GetPlaceables(new WorldLocation(5, 5)).Count() == 0);
            Assert.IsTrue(GameWorld.Placeables.GetPlaceables(new WorldLocation(6, 6)).Count() == 0);
            Assert.IsTrue(GameWorld.Placeables.GetPlaceables(new WorldLocation(7, 7)).Count() == 0);
        }
    }
}
