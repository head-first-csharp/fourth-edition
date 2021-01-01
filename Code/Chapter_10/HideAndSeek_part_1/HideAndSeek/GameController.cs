using System;
using System.Collections.Generic;
using System.Text;

namespace HideAndSeek
{
    public class GameController
    {
        private House house = new House();

        /// <summary>
        /// The player's current location in the house
        /// </summary>
        public Location CurrentLocation { get; private set; }

        /// <summary>
        /// Returns the the current status to show to the player
        /// </summary>
        public string Status => $@"You are in the {CurrentLocation}. You see the following exits:
 - {string.Join(Environment.NewLine + " - ", CurrentLocation.ExitList)}";

        /// <summary>
        /// A prompt to display to the player
        /// </summary>
        public const string Prompt = "Which direction do you want to go: ";

        public GameController()
        {
            CurrentLocation = house.Entry;
        }

        /// <summary>
        /// Move to the location in a direction
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        public bool Move(Direction direction)
        {
            var oldLocation = CurrentLocation;
            CurrentLocation = CurrentLocation.GetExit(direction);
            return (oldLocation != CurrentLocation);
        }

        /// <summary>
        /// Parses input from the player and updates the status
        /// </summary>
        /// <param name="input">Input to parse</param>
        /// <returns>The results of parsing the input</returns>
        public string ParseInput(string input)
        {
            var results = "That's not a valid direction";
            if (Enum.TryParse(typeof(Direction), input, out object direction))
            {
                if (!Move((Direction)direction))
                    results = "There's no exit in that direction";
                else
                    results = $"Moving {direction}";
            }
            return results;
        }
    }
}
