using System;
using System.Collections.Generic;
using System.Text;

namespace BasketballRoster.ViewModel
{
    using Model;
    using System.Collections.ObjectModel;

    class LeagueViewModel
    {
        public RosterViewModel AnasTeam { get; set; }
        public RosterViewModel JimmysTeam { get; set; }

        public LeagueViewModel()
        {
            var anasRoster = new Roster("The Bombers", GetBomberPlayers());
            AnasTeam = new RosterViewModel(anasRoster);

            var jimmysRoster = new Roster("The Amazins", GetAmazinPlayers());
            JimmysTeam = new RosterViewModel(jimmysRoster);
        }

        private IEnumerable<Player> GetBomberPlayers()
        {
            return new List<Player>() {
                new Player("Ana", 31, true),
                new Player("Lloyd", 23, true),
                new Player("Kathleen",6, true),
                new Player("Mike", 0, true),
                new Player("Joe", 42, true),
                new Player("Herb",32, false),
                new Player("Fingers",8, false),
            };
        }

        private IEnumerable<Player> GetAmazinPlayers()
        {
            return new List<Player>() {
                new Player("Jimmy",42, true),
                new Player("Henry",11, true),
                new Player("Bob",4, true),
                new Player("Lucinda", 18, true),
                new Player("Kim", 16, true),
                new Player("Bertha", 23, false),
                new Player("Ed",21,  false),
            };
        }
    }
}