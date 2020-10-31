using Newtonsoft.Json;

namespace DnDTrackerAPI.Models
{
    public class Monster
    {
        public int WaveId { get; set; }
        public string Index { get; set; }
        public int Count { get; set; }
        [JsonIgnore]
        public virtual Wave Wave { get; set; }

        public Monster(string index)
        {
            Index = index;
        }

        public Monster(string index, int waveId)
            : this(index)
        {
            WaveId = waveId;
        }

        public Monster(string index, int waveId, int count)
            : this(index, waveId)
        {
            Count = count;
        }
    }
}