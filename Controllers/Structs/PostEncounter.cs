using DnDTrackerAPI.Models;
using System.Collections.Generic;

namespace DnDTrackerAPI.Controllers
{
    public struct PostEncounter
    {
        public int UserId { get; set; }
        public string EncounterName { get; set; }
        public List<Wave> Waves { get; set; }
    }
}