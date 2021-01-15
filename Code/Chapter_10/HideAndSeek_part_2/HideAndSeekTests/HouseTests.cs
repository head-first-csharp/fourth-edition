using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HideAndSeekTests
{
    using HideAndSeek;
    using System.Linq;

    [TestClass]
    public class HouseTests
    {

        [TestMethod]
        public void TestLayout()
        {
            Assert.AreEqual("Entry", House.Entry.Name);

            var garage = House.Entry.GetExit(Direction.Out);
            Assert.AreEqual("Garage", garage.Name);

            var hallway = House.Entry.GetExit(Direction.East);
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

        [TestMethod]
        public void TestGetLocationByName()
        {
            Assert.AreEqual("Entry", House.GetLocationByName("Entry").Name);
            Assert.AreEqual("Attic", House.GetLocationByName("Attic").Name);
            Assert.AreEqual("Garage", House.GetLocationByName("Garage").Name);
            Assert.AreEqual("Master Bedroom", House.GetLocationByName("Master Bedroom").Name);
            Assert.AreEqual("Entry", House.GetLocationByName("Secret Library").Name);
        }

        [TestMethod]
        public void TestRandomExit()
        {
            var landing = House.GetLocationByName("Landing");

            House.Random = new MockRandom() { ValueToReturn = 0 };
            Assert.AreEqual("Attic", House.RandomExit(landing).Name);

            House.Random = new MockRandom() { ValueToReturn = 1 };
            Assert.AreEqual("Hallway", House.RandomExit(landing).Name);

            House.Random = new MockRandom() { ValueToReturn = 2 };
            Assert.AreEqual("Kids Room", House.RandomExit(landing).Name);

            House.Random = new MockRandom() { ValueToReturn = 3 };
            Assert.AreEqual("Master Bedroom", House.RandomExit(landing).Name);

            House.Random = new MockRandom() { ValueToReturn = 4 };
            Assert.AreEqual("Nursery", House.RandomExit(landing).Name);

            House.Random = new MockRandom() { ValueToReturn = 5 };
            Assert.AreEqual("Pantry", House.RandomExit(landing).Name);

            House.Random = new MockRandom() { ValueToReturn = 6 };
            Assert.AreEqual("Second Bathroom", House.RandomExit(landing).Name);

            var kitchen = House.GetLocationByName("Kitchen");
            House.Random = new MockRandom() { ValueToReturn = 0 };
            Assert.AreEqual("Hallway", House.RandomExit(kitchen).Name);
        }

        [TestMethod]
        public void TestHidingPlaces()
        {
            Assert.IsInstanceOfType(House.GetLocationByName("Garage"), typeof(LocationWithHidingPlace));
            Assert.IsInstanceOfType(House.GetLocationByName("Kitchen"), typeof(LocationWithHidingPlace));
            Assert.IsInstanceOfType(House.GetLocationByName("Living Room"), typeof(LocationWithHidingPlace));
            Assert.IsInstanceOfType(House.GetLocationByName("Bathroom"), typeof(LocationWithHidingPlace));
            Assert.IsInstanceOfType(House.GetLocationByName("Master Bedroom"),
                                                                     typeof(LocationWithHidingPlace));
            Assert.IsInstanceOfType(House.GetLocationByName("Master Bath"), typeof(LocationWithHidingPlace));
            Assert.IsInstanceOfType(House.GetLocationByName("Second Bathroom"),
                                                                     typeof(LocationWithHidingPlace));
            Assert.IsInstanceOfType(House.GetLocationByName("Kids Room"), typeof(LocationWithHidingPlace));
            Assert.IsInstanceOfType(House.GetLocationByName("Nursery"), typeof(LocationWithHidingPlace));
            Assert.IsInstanceOfType(House.GetLocationByName("Pantry"), typeof(LocationWithHidingPlace));
            Assert.IsInstanceOfType(House.GetLocationByName("Attic"), typeof(LocationWithHidingPlace));
        }

        [TestMethod]
        public void TestClearHidingPlaces()
        {
            var garage = House.GetLocationByName("Garage") as LocationWithHidingPlace;
            garage.Hide(new Opponent("Opponent1"));

            var attic = House.GetLocationByName("Garage") as LocationWithHidingPlace;
            attic.Hide(new Opponent("Opponent2"));
            attic.Hide(new Opponent("Opponent3"));
            attic.Hide(new Opponent("Opponent4"));


            House.ClearHidingPlaces();
            Assert.AreEqual(0, garage.CheckHidingPlace().Count());
            Assert.AreEqual(0, attic.CheckHidingPlace().Count());
        }

    }
}