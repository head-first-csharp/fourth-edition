using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HideAndSeekTests
{
    using HideAndSeek;
    using System;
    using System.Linq;
    using System.IO;

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
                            Environment.NewLine + " - the Landing is Up" +
                            Environment.NewLine + "You have not found any opponents", gameController.Status);

            Assert.AreEqual("Moving South", gameController.ParseInput("South"));
            Assert.AreEqual("You are in the Living Room. You see the following exits:" +
                            Environment.NewLine + " - the Hallway is to the North" +
                            Environment.NewLine + "Someone could hide behind the sofa" +
                            Environment.NewLine + "You have not found any opponents", gameController.Status);
        }

        [TestMethod]
        public void TestParseCheck()
        {
            Assert.IsFalse(gameController.GameOver);

            // Clear the hiding places and hide the opponents in specific rooms
            House.ClearHidingPlaces();
            var joe = gameController.Opponents.ToList()[0];
            (House.GetLocationByName("Garage") as LocationWithHidingPlace).Hide(joe);
            var bob = gameController.Opponents.ToList()[1];
            (House.GetLocationByName("Kitchen") as LocationWithHidingPlace).Hide(bob);
            var ana = gameController.Opponents.ToList()[2];
            (House.GetLocationByName("Attic") as LocationWithHidingPlace).Hide(ana);
            var owen = gameController.Opponents.ToList()[3];
            (House.GetLocationByName("Attic") as LocationWithHidingPlace).Hide(owen);
            var jimmy = gameController.Opponents.ToList()[4];
            (House.GetLocationByName("Kitchen") as LocationWithHidingPlace).Hide(jimmy);

            // Check the Entry -- there are no players hiding there
            Assert.AreEqual(1, gameController.MoveNumber);
            Assert.AreEqual("There is no hiding place in the Entry",
                            gameController.ParseInput("Check"));
            Assert.AreEqual(2, gameController.MoveNumber);

            // Move to the Garage
            gameController.ParseInput("Out");
            Assert.AreEqual(3, gameController.MoveNumber);

            // We hid Joe in the Garage, so validate ParseInput's return value and the properties
            Assert.AreEqual("You found 1 opponent hiding behind the car",
                            gameController.ParseInput("check"));
            Assert.AreEqual("You are in the Garage. You see the following exits:" +
                               Environment.NewLine + " - the Entry is In" +
                               Environment.NewLine + "Someone could hide behind the car" +
                               Environment.NewLine + "You have found 1 of 5 opponents: Joe",
                            gameController.Status);
            Assert.AreEqual("4: Which direction do you want to go (or type 'check'): ",
                            gameController.Prompt);
            Assert.AreEqual(4, gameController.MoveNumber);

            // Move to the bathroom, where nobody is hiding
            gameController.ParseInput("In");
            gameController.ParseInput("East");
            gameController.ParseInput("North");
            // Check the Bathroom to make sure nobody is hiding there
            Assert.AreEqual("Nobody was hiding behind the door", gameController.ParseInput("check"));
            Assert.AreEqual(8, gameController.MoveNumber);

            // Check the Bathroom to make sure nobody is hiding there
            gameController.ParseInput("South");
            gameController.ParseInput("Northwest");
            Assert.AreEqual("You found 2 opponents hiding next to the stove",
                            gameController.ParseInput("check"));
            Assert.AreEqual("You are in the Kitchen. You see the following exits:" +
                             Environment.NewLine + " - the Hallway is to the Southeast" +
                             Environment.NewLine + "Someone could hide next to the stove" +
                             Environment.NewLine + "You have found 3 of 5 opponents: Joe, Bob, Jimmy",
                            gameController.Status);
            Assert.AreEqual("11: Which direction do you want to go (or type 'check'): ",
                            gameController.Prompt);
            Assert.AreEqual(11, gameController.MoveNumber);

            Assert.IsFalse(gameController.GameOver);

            // Head up to the Landing, then check the Pantry (nobody's hiding there)
            gameController.ParseInput("Southeast");
            gameController.ParseInput("Up");
            Assert.AreEqual(13, gameController.MoveNumber);

            gameController.ParseInput("South");
            Assert.AreEqual("Nobody was hiding inside a cabinet",
                            gameController.ParseInput("check"));
            Assert.AreEqual(15, gameController.MoveNumber);

            // Check the Attic to find the last two opponents, make sure the game is over
            gameController.ParseInput("North");
            gameController.ParseInput("Up");
            Assert.AreEqual(17, gameController.MoveNumber);

            Assert.AreEqual("You found 2 opponents hiding in a trunk",
                            gameController.ParseInput("check"));
            Assert.AreEqual("You are in the Attic. You see the following exits:" +
                               Environment.NewLine + " - the Landing is Down" +
                               Environment.NewLine + "Someone could hide in a trunk" +
                               Environment.NewLine +
                               "You have found 5 of 5 opponents: Joe, Bob, Jimmy, Ana, Owen",
                            gameController.Status);
            Assert.AreEqual("18: Which direction do you want to go (or type 'check'): ",
                            gameController.Prompt);
            Assert.AreEqual(18, gameController.MoveNumber);

            Assert.IsTrue(gameController.GameOver);
        }

        [TestMethod]
        public void TestSaveAndLoad()
        {
            // Clear the hiding places and hide the opponents in specific rooms
            House.ClearHidingPlaces();
            var joe = gameController.Opponents.ToList()[0];
            (House.GetLocationByName("Garage") as LocationWithHidingPlace).Hide(joe);
            var bob = gameController.Opponents.ToList()[1];
            (House.GetLocationByName("Garage") as LocationWithHidingPlace).Hide(bob);
            var ana = gameController.Opponents.ToList()[2];
            (House.GetLocationByName("Attic") as LocationWithHidingPlace).Hide(ana);
            var owen = gameController.Opponents.ToList()[3];
            (House.GetLocationByName("Attic") as LocationWithHidingPlace).Hide(owen);
            var jimmy = gameController.Opponents.ToList()[4];
            (House.GetLocationByName("Kitchen") as LocationWithHidingPlace).Hide(jimmy);

            // Find three opponents and move to the Hallway
            gameController.ParseInput("Out");
            gameController.ParseInput("Check");
            gameController.ParseInput("In");
            gameController.ParseInput("East");
            gameController.ParseInput("Northwest");
            gameController.ParseInput("Check");
            gameController.ParseInput("Southeast");
            Assert.AreEqual(8, gameController.MoveNumber);
            Assert.AreEqual("Hallway", gameController.CurrentLocation.Name);

            string filename;
            do
            {
                filename = $"testsave_{new Random().Next()}";
            } while (File.Exists($"{filename}.json"));

            // Save the game state to a temporary file
            gameController.ParseInput($"save {filename}");

            gameController = new GameController();
            Assert.AreEqual(1, gameController.MoveNumber);
            Assert.AreEqual("Entry", gameController.CurrentLocation.Name);

            gameController.ParseInput($"load {filename}");
            Assert.AreEqual(8, gameController.MoveNumber);
            Assert.AreEqual("Hallway", gameController.CurrentLocation.Name);
            Assert.AreEqual("You are in the Hallway. You see the following exits:" +
                   Environment.NewLine + " - the Bathroom is to the North" +
                   Environment.NewLine + " - the Living Room is to the South" +
                   Environment.NewLine + " - the Entry is to the West" +
                   Environment.NewLine + " - the Kitchen is to the Northwest" +
                   Environment.NewLine + " - the Landing is Up" +
                   Environment.NewLine + "You have found 3 of 5 opponents: Joe, Bob, Jimmy",
                gameController.Status);

            // Clean up your temporary file
            File.Delete(filename);
            Assert.IsTrue(!File.Exists(filename));
        }

        [TestMethod]
        public void TestInvalidFilenames()
        {
            Assert.AreEqual("Please enter a filename without slashes or spaces.", gameController.Save("invalid\\filename"));
            Assert.AreEqual("Please enter a filename without slashes or spaces.", gameController.Save("invalid/filename"));
            Assert.AreEqual("Please enter a filename without slashes or spaces.", gameController.Save("invalid filename"));
        }
    }
}