using System;
using System.Collections.Generic;
using System.Text;

namespace HideAndSeek
{
    using System.Linq;

    public class GameController
    {
        /// <summary>
        /// The player's current location in the house
        /// </summary>
        public Location CurrentLocation { get; private set; }

        /// <summary>
        /// Returns the the current status to show to the player
        /// </summary>
        public string Status
        {
            get
            {
                var found = foundOpponents.Count() == 0 ? "You have not found any opponents"
                    : $"You have found {foundOpponents.Count()} of {Opponents.Count()} opponents: {string.Join(", ", foundOpponents.Select(o => o.Name))}";
                var hidingPlace = (CurrentLocation is LocationWithHidingPlace location) ?
                    $"{Environment.NewLine}Someone could hide {location.HidingPlace}" : "";
                return $"You are in the {CurrentLocation}. You see the following exits:" + Environment.NewLine +
                    $" - {string.Join(Environment.NewLine + " - ", CurrentLocation.ExitList)}{hidingPlace}" +
                    $"{Environment.NewLine}{found}";
            }
        }
        /// <summary>
        /// The number of moves the player has made
        /// </summary>
        public int MoveNumber { get; private set; } = 1;

        /// <summary>
        /// Private list of opponents the player needs to find
        /// </summary>
        public readonly IEnumerable<Opponent> Opponents = new List<Opponent>()
        {
            new Opponent("Joe"),
            new Opponent("Bob"),
            new Opponent("Ana"),
            new Opponent("Owen"),
            new Opponent("Jimmy"),
        };

        /// <summary>
        /// Private list of opponents the player has found so far
        /// </summary>
        private readonly List<Opponent> foundOpponents = new List<Opponent>();

        /// <summary>
        /// Returns true if the game is over
        /// </summary>
        public bool GameOver => Opponents.Count() == foundOpponents.Count();

        /// <summary>
        /// A prompt to display to the player
        /// </summary>
        public string Prompt => $"{MoveNumber}: Which direction do you want to go (or type 'check'): ";

        public GameController()
        {
            House.ClearHidingPlaces();
            foreach (var opponent in Opponents)
                opponent.Hide();

            CurrentLocation = House.Entry;
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

            if (input.ToLower() == "check")
            {
                MoveNumber++;
                if (CurrentLocation is LocationWithHidingPlace locationWithHidingPlace)
                {
                    var found = locationWithHidingPlace.CheckHidingPlace();
                    if (found.Count() == 0)
                        results = $"Nobody was hiding {locationWithHidingPlace.HidingPlace}";
                    else
                    {
                        foundOpponents.AddRange(found);
                        var s = found.Count() == 1 ? "" : "s";
                        results = $"You found {found.Count()} opponent{s} hiding {locationWithHidingPlace.HidingPlace}";
                    }
                }
                else
                {
                    results = $"There is no hiding place in the {CurrentLocation}";
                }
            }

            else if (Enum.TryParse(typeof(Direction), input, out object direction))
            {
                if (!Move((Direction)direction))
                    results = "There's no exit in that direction";
                else
                {
                    MoveNumber++;
                    results = $"Moving {direction}";
                }
            }
            return results;
        }


    }
}
