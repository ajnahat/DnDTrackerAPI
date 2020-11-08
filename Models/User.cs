using System.Collections.Generic;
using Newtonsoft.Json;

namespace DnDTrackerAPI.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public virtual List<Encounter> Encounters { get; set; }
    }
}