using System;
using System.Collections.Generic;
using System.Text;

namespace HideAndSeek
{
    public static class House
    {
        /// <summary>
        /// Returns the starting location for the player
        /// </summary>
        public static readonly Location Entry;

        /// <summary>
        /// Sets up the house data structure
        /// </summary>
        static House()
        {
            Entry = new Location("Entry");
            var hallway = new Location("Hallway");
            var livingRoom = new Location("Living Room");
            var kitchen = new Location("Kitchen");
            var bathroom = new Location("Bathroom");
            var landing = new Location("Landing");
            var masterBedroom = new Location("Master Bedroom");
            var masterBath = new Location("Master Bath");
            var secondBathroom = new Location("Second Bathroom");
            var kidsRoom = new Location("Kids Room");
            var nursery = new Location("Nursery");
            var pantry = new Location("Pantry");
            var attic = new Location("Attic");
            var garage = new Location("Garage");

            Entry.AddExit(Direction.East, hallway);
            Entry.AddExit(Direction.Out, garage);
            hallway.AddExit(Direction.Northwest, kitchen);
            hallway.AddExit(Direction.North, bathroom);
            hallway.AddExit(Direction.South, livingRoom);
            hallway.AddExit(Direction.Up, landing);
            landing.AddExit(Direction.Northwest, masterBedroom);
            landing.AddExit(Direction.West, secondBathroom);
            landing.AddExit(Direction.Southwest, nursery);
            landing.AddExit(Direction.South, pantry);
            landing.AddExit(Direction.Southeast, kidsRoom);
            landing.AddExit(Direction.Up, attic);
            masterBedroom.AddExit(Direction.East, masterBath);
        }
    }
}
