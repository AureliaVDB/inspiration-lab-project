using System;
using System.Collections.Generic;

namespace KeepTrackApp.Models
{
    public class Progress
    {
        public string UserId { get; set; }
        public List<(DateTime Day, float Weight)> WeightEntries { get; set; } = new();

        public void AddWeight(DateTime day, float kg) => WeightEntries.Add((day, kg));

        public Dictionary<string, float> CalculateWeeklyAverages()
        {
            return new Dictionary<string, float>(); // Simplified
        }

        public object GenerateMacroGraph()
        {
            return new {}; // Placeholder
        }

        public Dictionary<string, float> RecommendMacros()
        {
            return new Dictionary<string, float>(); // Placeholder
        }
    }
}
