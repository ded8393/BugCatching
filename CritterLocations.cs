using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

using StardewValley;
using Critter = StardewValley.BellsAndWhistles.Critter;
using StardewValley.Locations;

using StardewModdingAPI;
using StardewModdingAPI.Framework.ModHelpers;

namespace BugNet
{
    public static class CritterLocations
    {
        private static IModHelper Helper;

        public static void init(IModHelper helper)
        {
            Helper = helper;
        }

        public static List<Critter> critterList;
        static public List<Critter> GetCrittersAtGameLocation(GameLocation location)
        {
            object privateFieldValue = location.GetType().GetField("critters", BindingFlags.NonPublic | BindingFlags.Instance)
                            .GetValue(location);
            return (List<Critter>)privateFieldValue;

        }
        public static void UpdateCrittersAtGameLocation(GameLocation location, List<Critter> UpdatedCritterList)
        {
            location.GetType().GetField("critters", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(location, UpdatedCritterList);
        }

        public static void RemoveCritterFromGameLocation(GameLocation location, Critter critter)
        {
            List<Critter> newCritterList = GetCrittersAtGameLocation(location);
            newCritterList.Remove(critter);
            UpdateCrittersAtGameLocation(location, newCritterList);
        }
        public static void AddCritterToGameLocation(GameLocation location, Critter critter)
        {
            List<Critter> newCritterList = GetCrittersAtGameLocation(location);
            newCritterList.Add(critter);
            UpdateCrittersAtGameLocation(location, newCritterList);
        }

        
        //public Dictionary<string, List<Critter>> getCritterDictionary(IModHelper helper)
        //{
        //    Dictionary<string, List<Critter>> critterDict = new Dictionary<string, List<Critter>>();
        //    foreach (GameLocation location in Game1.locations)
        //    {
        //        List<Critter> critterList = helper.Reflection.GetField<List<Critter>>(location, "critter").GetValue();
        //        critterDict.Add(location.name, critterList);
        //    }
        //    return critterDict;
        //}

    }
    
}
