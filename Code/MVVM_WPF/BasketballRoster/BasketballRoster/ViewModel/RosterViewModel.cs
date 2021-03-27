using System;
using System.Collections.Generic;
using System.Text;

namespace BasketballRoster.ViewModel
{
    using Model;
    using System.Collections.ObjectModel;
    using System.Linq;

    class RosterViewModel
    {
        public ObservableCollection<PlayerViewModel> Starters { get; set; }
        public ObservableCollection<PlayerViewModel> Bench { get; set; }

        private Roster _roster;


        private string _teamName;
        public string TeamName
        {
            get { return _teamName; }
            set { _teamName = value; }
        }

        public RosterViewModel(Roster roster)
        {
            _roster = roster;

            Starters = new ObservableCollection<PlayerViewModel>();
            Bench = new ObservableCollection<PlayerViewModel>();

            TeamName = _roster.TeamName;

            UpdateRosters();
        }

        private void UpdateRosters()
        {
            var startingPlayers = _roster.Players
                .Where(player => player.Starter)
                .Select(player => new PlayerViewModel(player.Name, player.Number));

            foreach (var playerViewModel in startingPlayers)
                Starters.Add(playerViewModel);

            var benchPlayers = _roster.Players
                .Where(player => !player.Starter)
                .Select(player => new PlayerViewModel(player.Name, player.Number));

            foreach (var playerViewModel in benchPlayers)
                Bench.Add(playerViewModel);
        }
    }
}