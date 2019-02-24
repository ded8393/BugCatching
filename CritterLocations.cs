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

namespace BugCatching
{
    public class CritterLocations
    {
        public GameLocation Location = new GameLocation();
        public int CritterCount;
        public List<Critter> critterList;

        private static IModHelper Helper;

        public static void init(IModHelper helper)
        {
            Helper = helper;
        }
        public CritterLocations(GameLocation gameLocation)
        {
            Location = Game1.getLocationFromName(gameLocation.Name);
            //todo check for return null
        }

        public List<Critter> GetCritters()
        {
            object privateFieldValue = this.Location.GetType().GetField("critters", BindingFlags.NonPublic | BindingFlags.Instance)
                            .GetValue(this.Location);
            return (List<Critter>)privateFieldValue;
        }
        public void UpdateCritters( List<Critter> UpdatedCritterList)
        {
            this.Location.GetType().GetField("critters", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(this.Location, UpdatedCritterList);
        }

        public void RemoveThisCritter(Critter critter)
        {
            List<Critter> newCritterList = GetCritters();
            newCritterList.Remove(critter);
            UpdateCritters(newCritterList);
        }
        //public void AddCritterToGameLocation(GameLocation location, Critter critter)
        //{
        //    location.addCritter(critter);
//        if (!this.largeTerrainFeatures[index2].getBoundingBox().Intersects(new Microsoft.Xna.Framework.Rectangle((int) tilePosition.X* 64, (int) tilePosition.Y * 64, 64, 64)) && !this.isTileLocationTotallyClearAndPlaceable(tilePosition))
//            {
//              flag = false;
//              break;
//            }
//}
//          if (flag)
//          {
//            this.critters.Add((Critter) new Rabbit(tilePosition, flip));
//            break;
//          }
        //}

        
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
