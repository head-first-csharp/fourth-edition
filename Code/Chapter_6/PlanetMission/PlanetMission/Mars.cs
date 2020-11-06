using System;
using System.Collections.Generic;
using System.Text;

namespace PlanetMission
{
    class Mars : PlanetMission
    {
        public Mars()
        {
            kmToPlanet = 92000000;
            fuelPerKm = 1.73f;
            kmPerHour = 37000;
        }
    }

}
