using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ScoreboardAPI.Models
{
    /// <summary>
    /// Represents a score entry
    /// </summary>
    public class Score
    {
        public int ID { get; set; }
        public int PlayerScore { get; set; }
        public string PlayerID { get; set; }
        public string GameName { get; set; } = "";
        public DateTime TimeStamp { get; set; }
    }
}