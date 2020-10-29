using System;
using System.Collections.Generic;

namespace IRCPlugin
{
    public class Player
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int TotalGamesPlayed { get; set; }
        public int TotalScpGamesPlayed { get; set; }
        public DateTime FirstJoin { get; set; }
        public DateTime LastSeen { get; set; }
        public int Exp { get; set; }
        public int Level { get; set; }
        public int TotalKilled { get; set; }
        public int TotalScpKilled { get; set; }
        public int TotalDeath { get; set; }
        public int TotalEscaped { get; set; }
        public int TotalWin {get; set;}
        public bool DisplayBadge { get; set; }
        public Dictionary<string, int> PlayTimeRecords { get; set; } = new Dictionary<string, int>();
        
        public void SetCurrentDayPlayTime()
        {
            if (!PlayTimeRecords.ContainsKey(DateTime.Now.Date.ToShortDateString())) PlayTimeRecords.Add(DateTime.Now.Date.ToShortDateString(), 0);
            PlayTimeRecords[DateTime.Now.Date.ToShortDateString()] += (int)(DateTime.Now - LastSeen).TotalSeconds;
        }
        
        public void Reset()
        {
            TotalScpGamesPlayed = 0;
            TotalScpGamesPlayed = 0;
            Exp = 0;
            Level = 1;
            TotalKilled = 0;
            TotalScpKilled = 0;
            TotalDeath = 0;
            TotalEscaped = 0;
            TotalWin = 0;
            DisplayBadge = false;
        }
    }
}