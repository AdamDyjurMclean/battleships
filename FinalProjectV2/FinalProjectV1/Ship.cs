using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProjectV1
{
    public class Ship
    {
        public string name;
        public string orientation = "none";
        public int width;
        public int hits;
        public int placed = 0;
        public List<string> locations = new List<string>();
        public bool Sunk
        {
            get
            {
                return hits >= width;
            }
        }
        public Ship(string name, int width)
        {
            this.name = name;
            this.width = width;
        }
        public void AddLocation(string location)
        {
            this.locations.Add(location);
        }
    }
}
