using Newtonsoft.Json;
using System.Collections.Generic;

namespace DnDTrackerAPI.Models
{
    public class Encounter
    {
        public int EncounterId { get; set; }
        public int UserId { get; set; }
        public string EncounterName { get; set; }
        public int Sort { get; set; }
        [JsonIgnore]
        public virtual User User { get; set; }
        public virtual List<Wave> Waves { get; set; }
    }
}