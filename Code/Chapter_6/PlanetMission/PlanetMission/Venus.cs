using System;
using System.Collections.Generic;
using System.Text;

namespace PlanetMission
{
    class Venus : PlanetMission
    {
        public Venus()
        {
            kmToPlanet = 41000000;
            fuelPerKm = 2.11f;
            kmPerHour = 29500;
        }
    }
}
