using System.Collections.Generic; 
namespace Dolores.Clients.Noaa.Models.Forecast{ 

    public class Geometry
    {
        public string type { get; set; }
        public List<List<List<double>>> coordinates { get; set; }
    }

}