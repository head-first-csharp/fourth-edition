using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HideAndSeekTests
{
    using HideAndSeek;
    using System.Collections.Generic;
    using System.Linq;

    [TestClass]
    public class LocationTests
    {
        private Location center;

        /// <summary>
        /// Initializes each unit test by setting createing a new the center location
        /// and adding a room in each direction before the test
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            center = new Location("Center Room");
            Assert.AreSame("Center Room", center.ToString());
            Assert.AreEqual(0, center.ExitList.Count());

            center.AddExit(Direction.North, new Location("North Room"));
            center.AddExit(Direction.Northeast, new Location("Northeast Room"));
            center.AddExit(Direction.East, new Location("East Room"));
            center.AddExit(Direction.Southeast, new Location("Southeast Room"));
            center.AddExit(Direction.South, new Location("South Room"));
            center.AddExit(Direction.Southwest, new Location("East Room"));
            center.AddExit(Direction.West, new Location("West Room"));
            center.AddExit(Direction.Northwest, new Location("Northwest Room"));
            center.AddExit(Direction.Up, new Location("Upper Room"));
            center.AddExit(Direction.Down, new Location("Lower Room"));
            center.AddExit(Direction.In, new Location("Inside Room"));
            center.AddExit(Direction.Out, new Location("Outside Room"));

            Assert.AreEqual(12, center.ExitList.Count());
        }

        /// <summary>
        /// Make sure GetExit returns the location in a direction only if it exists
        /// </summary>
        [TestMethod]
        public void TestGetExit()
        {
            var eastRoom = center.GetExit(Direction.East);
            Assert.AreEqual("East Room", eastRoom.Name);
            Assert.AreSame(center, eastRoom.GetExit(Direction.West));
            Assert.AreSame(eastRoom, eastRoom.GetExit(Direction.Up));
        }

        /// <summary>
        /// Validates that the exit lists are working
        /// </summary>
        [TestMethod]
        public void TestExitList()
        {
            CollectionAssert.AreEqual(
               new List<string>() {
                   "the North Room is to the North",
                   "the South Room is to the South",
                   "the East Room is to the East",
                   "the West Room is to the West",
                   "the Northeast Room is to the Northeast",
                   "the East Room is to the Southwest",
                   "the Southeast Room is to the Southeast",
                   "the Northwest Room is to the Northwest",
                   "the Upper Room is Up",
                   "the Lower Room is Down",
                   "the Inside Room is In",
                   "the Outside Room is Out",
               },
               center.ExitList.ToList());
        }

        /// <summary>
        /// Validates that each room's name and return exit is created correctly
        /// </summary>
        [TestMethod]
        public void TestReturnExits()
        {
            var e = center.GetExit(Direction.East);
            Assert.AreEqual("East Room", e.ToString());
            Assert.AreSame(center, e.GetExit(Direction.West));
            Assert.AreEqual(1, e.ExitList.Count());

            var nw = center.GetExit(Direction.Northwest);
            Assert.AreEqual("Northwest Room", nw.ToString());
            Assert.AreSame(center, nw.GetExit(Direction.Southeast));

            var se = center.GetExit(Direction.Southeast);
            Assert.AreEqual("Southeast Room", se.ToString());
            Assert.AreSame(center, se.GetExit(Direction.Northwest));

            var s = center.GetExit(Direction.South);
            Assert.AreEqual("South Room", s.ToString());
            Assert.AreSame(center, s.GetExit(Direction.North));

            var up = center.GetExit(Direction.Up);
            Assert.AreEqual("Upper Room", up.ToString());
            Assert.AreSame(center, up.GetExit(Direction.Down));

            var outside = center.GetExit(Direction.Out);
            Assert.AreEqual("Outside Room", outside.ToString());
            Assert.AreSame(center, outside.GetExit(Direction.In));
        }

        /// <summary>
        /// Add a hall to one of the rooms and make sure the hall room's names
        /// and return exits are created correctly
        /// </summary>
        [TestMethod]
        public void TestAddHall()
        {
            var e = center.GetExit(Direction.East);
            Assert.AreEqual(1, e.ExitList.Count());
            var eastHall1 = new Location("East hall 1");
            var eastHall2 = new Location("East hall 2");
            e.AddExit(Direction.East, eastHall1);
            eastHall1.AddExit(Direction.East, eastHall2);
            Assert.AreEqual(2, e.ExitList.Count());
            Assert.AreEqual(2, eastHall1.ExitList.Count());
            Assert.AreEqual(1, eastHall2.ExitList.Count());
        }
    }
}
