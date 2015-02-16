using System;
using exd.World;
using exd.World.Buildings;
using exd.World.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExdUnitTests
{
    [TestClass]
    public class BuildingTests
    {
        [TestMethod]
        public void BasicBuildingPlacement()
        {
            GameWorld.Initialize(1000, 1000, 1);

            Assert.IsTrue(GameWorld.Placeables.Add(new Storage(new WorldLocation(5, 5), new WorldDimension(15, 15))));
            Assert.IsTrue(GameWorld.Placeables.Add(new Storage(new WorldLocation(21, 5), new WorldDimension(15, 15))));
            Assert.IsFalse(GameWorld.Placeables.Add(new Storage(new WorldLocation(20, 20), new WorldDimension(2, 2))));
        }
    }
}
