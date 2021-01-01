using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HideAndSeekTests
{
    using HideAndSeek;

    [TestClass]
    public class HouseTests
    {
        House house;

        [TestInitialize]
        public void Initialize()
        {
            house = new House();
        }

        [TestMethod]
        public void TestLayout()
        {
            Assert.AreEqual("Entry", house.Entry.Name);

            var garage = house.Entry.GetExit(Direction.Out);
            Assert.AreEqual("Garage", garage.Name);

            var hallway = house.Entry.GetExit(Direction.East);
            Assert.AreEqual("Hallway", hallway.Name);

            var kitchen = hallway.GetExit(Direction.Northwest);
            Assert.AreEqual("Kitchen", kitchen.Name);

            var bathroom = hallway.GetExit(Direction.North);
            Assert.AreEqual("Bathroom", bathroom.Name);

            var livingRoom = hallway.GetExit(Direction.South);
            Assert.AreEqual("Living Room", livingRoom.Name);

            var landing = hallway.GetExit(Direction.Up);
            Assert.AreEqual("Landing", landing.Name);

            var masterBedroom = landing.GetExit(Direction.Northwest);
            Assert.AreEqual("Master Bedroom", masterBedroom.Name);

            var masterBath = masterBedroom.GetExit(Direction.East);
            Assert.AreEqual("Master Bath", masterBath.Name);

            var secondBathroom = landing.GetExit(Direction.West);
            Assert.AreEqual("Second Bathroom", secondBathroom.Name);

            var nursery = landing.GetExit(Direction.Southwest);
            Assert.AreEqual("Nursery", nursery.Name);

            var pantry = landing.GetExit(Direction.South);
            Assert.AreEqual("Pantry", pantry.Name);

            var kidsRoom = landing.GetExit(Direction.Southeast);
            Assert.AreEqual("Kids Room", kidsRoom.Name);

            var attic = landing.GetExit(Direction.Up);
            Assert.AreEqual("Attic", attic.Name);
        }
    }
}