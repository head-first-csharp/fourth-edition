using System;
using System.Collections.Generic;
using System.Text;

namespace HideAndSeek
{
    class SavedGame
    {
        public string PlayerLocation { get; set; }
        public Dictionary<String, String> OpponentLocations { get; set; }
        public List<String> FoundOpponents { get; set; }
        public int MoveNumber { get; set; }
    }
}
