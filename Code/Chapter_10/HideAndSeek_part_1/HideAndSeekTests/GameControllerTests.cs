using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HideAndSeekTests
{
    using HideAndSeek;
    using System;

    [TestClass]
    public class GameControllerTests
    {
        GameController gameController;

        [TestInitialize]
        public void Initialize()
        {
            gameController = new GameController();
        }

        [TestMethod]
        public void TestMovement()
        {
            Assert.AreEqual("Entry", gameController.CurrentLocation.Name);

            Assert.IsFalse(gameController.Move(Direction.Up));
            Assert.AreEqual("Entry", gameController.CurrentLocation.Name);

            Assert.IsTrue(gameController.Move(Direction.East));
            Assert.AreEqual("Hallway", gameController.CurrentLocation.Name);

            Assert.IsTrue(gameController.Move(Direction.Up));
            Assert.AreEqual("Landing", gameController.CurrentLocation.Name);

            // Add more movement tests to the TestMovement test method
        }

        [TestMethod]
        public void TestParseInput()
        {
            var initialStatus = gameController.Status;

            Assert.AreEqual("That's not a valid direction", gameController.ParseInput("X"));
            Assert.AreEqual(initialStatus, gameController.Status);

            Assert.AreEqual("There's no exit in that direction",
                            gameController.ParseInput("Up"));
            Assert.AreEqual(initialStatus, gameController.Status);

            Assert.AreEqual("Moving East", gameController.ParseInput("East"));
            Assert.AreEqual("You are in the Hallway. You see the following exits:" +
                            Environment.NewLine + " - the Bathroom is to the North" +
                            Environment.NewLine + " - the Living Room is to the South" +
                            Environment.NewLine + " - the Entry is to the West" +
                            Environment.NewLine + " - the Kitchen is to the Northwest" +
                            Environment.NewLine + " - the Landing is Up", gameController.Status);

            Assert.AreEqual("Moving South", gameController.ParseInput("South"));
            Assert.AreEqual("You are in the Living Room. You see the following exits:" +
                            Environment.NewLine + " - the Hallway is to the North", gameController.Status);

            // Add more input parsing tests to the TestParseInput test method
        }
    }
}