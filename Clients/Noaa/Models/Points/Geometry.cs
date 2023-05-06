using System.Collections.Generic; 
namespace Dolores.Clients.Noaa.Models.Points
{ 

    public class Geometry
    {
        public string type { get; set; }
        public List<double> coordinates { get; set; }
    }

}