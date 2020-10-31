using Newtonsoft.Json;
using System.Collections.Generic;

namespace DnDTrackerAPI.Models
{
    public class Wave
    {
        public int EncounterId { get; set; }
        public int WaveId { get; set; }
        public int Sort { get; set; }
        public virtual List<Monster> Monsters { get; set; }

        [JsonIgnore]
        public virtual Encounter Encounter { get; set; }
    }
}