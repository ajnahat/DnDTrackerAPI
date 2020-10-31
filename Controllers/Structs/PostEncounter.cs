using DnDTrackerAPI.Objects;
using System.Collections.Generic;

namespace DnDTrackerAPI.Controllers
{
    public struct PostEncounter
    {
        public int UserId { get; set; }
        public string EncounterName { get; set; }
        public List<List<Countable<string>>> Waves { get; set; }
    }
}