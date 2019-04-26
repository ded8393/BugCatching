using StardewValley;

namespace BugCatching
{
    public class SpawnConditions
    {
        public string[] Seasons { get; set; } = new string[0];
        public string[] Locations { get; set; } = new string[0];

        public int MinTimeOfDay { get; set; } = -1;
        public int MaxTimeOfDay { get; set; } = -1;
        public bool RequireDarkOut { get; set; } = false;
        public bool AllowRain { get; set; } = false;

        public bool allConditionsMet()
        {
            if (MinTimeOfDay != -1 && Game1.timeOfDay < MinTimeOfDay)
                return false;
            else if (MaxTimeOfDay != -1 && Game1.timeOfDay > MaxTimeOfDay)
                return false;
            else if (RequireDarkOut && !Game1.isDarkOut())
                return false;
            else if (!AllowRain && Game1.isRaining)
                return false;
            return true;
        }
    }
}
