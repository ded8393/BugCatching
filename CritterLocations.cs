using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.Xna.Framework;

using StardewValley;
using Critter = StardewValley.BellsAndWhistles.Critter;
using StardewValley.TerrainFeatures;

using StardewModdingAPI;
using StardewModdingAPI.Framework.ModHelpers;

using PyTK.Extensions;
using PyTK.Types;

namespace BugCatching
{
    public class CritterLocations
    {
        public GameLocation Location = new GameLocation();
        public int CritterCount;
        public List<Critter> critterList;
        public static List<CritterLocation> CritterHomes = new List<CritterLocation>();

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
        public void AddDiggableCritterToTerrainFeature(CritterEntry critter, Vector2 tilePosition, string layerName)
        {
            //bool flag = true;
            int bugIndex = critter.BugModel.ParentSheetIndex;
            TerrainFeature terrainFeature = this.Location.terrainFeatures[tilePosition];
            Log.debug($"Terrain feature {terrainFeature.GetType()}");
            //Location.setTileProperty((int)tilePosition.X, (int)tilePosition.Y, "Back", "Diggable", "T");
            //Location.setTileProperty((int)tilePosition.X, (int)tilePosition.Y, "Back", "Passable", "T");
            //Location.setTileProperty((int)tilePosition.X, (int)tilePosition.Y, "Back", "Treasure", $"Object {bugIndex}");
            //var sObject = new StardewValley.Object(tilePosition, bugIndex, 0);
            //Location.objects.Add(tilePosition, sObject);
            CritterLocation critterLocation = new CritterLocation() { location = Location, layerName = layerName, tilePosition = tilePosition, CritterEntry = critter };
            Log.info($"Registering critterLocation {critterLocation.location} {critterLocation.layerName} {critterLocation.tilePosition} {critterLocation.CritterEntry}");
            CritterHomes.AddOrReplace(critterLocation);

            new TileAction("disturbBug", DisturbBug).register();
            //new TerrainSelector<TerrainFeature>(t => t== terrainFeature).whenAddedToLocation(TileAction.getCustomAction("disturbBug"));
            this.Location.Map.addTouchAction(tilePosition, "disturbBug", "");
            //TileAction stepOnFeature = new TileAction("DisturbBug", this.Location, tilePosition, "back" )
            
            

            //this.Location.isTileLocationTotallyClearAndPlaceable(tilePosition)
           // ButtonClick.UseToolButton.onTerrainClick<TerrainFeature>(TileAction.getCustomAction("")));
        }
        public bool DisturbBug(string action, GameLocation location, Vector2 tile, string layerName)
        {
            Log.debug($"{action} | {location} | {tile} | {layerName}");
            bool flag = false;
            CritterLocation locationEntry = new CritterLocation();
            CritterEntry critterEntry = new CritterEntry();
            //foreach(CritterLocation cL in CritterHomes)
            //    Log.info($"{cL.location} {cL.tilePosition} {cL.layerName}");
            
            locationEntry = CritterHomes.Find(c => c.layerName == layerName && c.location == location && c.tilePosition == tile);
            if (locationEntry != null)
            {
                Log.info("locationEntry found");
                critterEntry = CritterEntry.critters.Find(ce => ce.Value == locationEntry.CritterEntry).Value;
                Location.addCritter(critterEntry.makeCritter(tile * Game1.tileSize));
                CritterHomes.Remove(locationEntry);
                flag = true;

            }
            Log.info("locationEntry not found");
            return flag;
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
    public class CritterLocation
    {
        public GameLocation location { get; set; } = new GameLocation();
        public string layerName { get; set; }
        public Vector2 tilePosition { get; set; } = new Vector2();
        public CritterEntry CritterEntry { get; set; } = new CritterEntry();
    }
}
