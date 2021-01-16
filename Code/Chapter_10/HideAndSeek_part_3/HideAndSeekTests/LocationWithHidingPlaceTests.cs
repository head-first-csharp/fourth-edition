using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HideAndSeekTests
{
    using HideAndSeek;
    using System.Collections.Generic;
    using System.Linq;

    [TestClass]
    public class LocationWithHidingPlaceTests
    {
        [TestMethod]
        public void TestHiding()
        {
            // The constructor sets the Name and HidingPlace properties
            var hidingLocation = new LocationWithHidingPlace("Room", "under the bed");
            Assert.AreEqual("Room", hidingLocation.Name);
            Assert.AreEqual("Room", hidingLocation.ToString());
            Assert.AreEqual("under the bed", hidingLocation.HidingPlace);

            // Hide two opponents in the room, then check the hiding place
            var opponent1 = new Opponent("Opponent1");
            var opponent2 = new Opponent("Opponent2");
            hidingLocation.Hide(opponent1);
            hidingLocation.Hide(opponent2);
            CollectionAssert.AreEqual(new List<Opponent>() { opponent1, opponent2 },
                hidingLocation.CheckHidingPlace().ToList());

            // The hiding place should now be empty
            CollectionAssert.AreEqual(new List<Opponent>(),
                hidingLocation.CheckHidingPlace().ToList());
        }
    }
}