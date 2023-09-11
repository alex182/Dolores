using System.ComponentModel.DataAnnotations;

namespace Dolores.Services.UkraineStats.Models
{
    public enum AssetCategory
    {
        [Display(Name ="Personnel")]
        Personnel = 0,         
        [Display(Name = "Tanks")]
        Tanks = 1,         
        [Display(Name = "APV")]
        APV = 2,         
        [Display(Name = "Artillery")]
        Artillery = 3,         
        [Display(Name = "MLRS")]
        MLRS = 4,         
        [Display(Name = "Anti-aircraft")]
        AntiAircraft = 5,         
        [Display(Name = "Aircraft")]
        Aircraft = 6,         
        [Display(Name = "Helicopters")]
        Helicopters = 7,         
        [Display(Name = "UAV")]
        UAV = 8,         
        [Display(Name = "Cruise Missiles")]
        CruiseMissiles = 9,         
        [Display(Name = "Warships and Boats")]
        WarshipsBoats = 10,         
        [Display(Name = "Vehicles and Fuel Tanks")]
        VehiclesAndFuelTanks = 11,
        [Display(Name = "Special Equipment")]
        SpecialEquipment = 12,        
        [Display(Name = "Uncategorized")]
        Uncategorized = 13

    }
}
