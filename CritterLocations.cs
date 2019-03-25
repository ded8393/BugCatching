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

using PyTK.CustomElementHandler;

using PyTK.Extensions;
using PyTK.Types;

namespace BugCatching
{
    public class CritterLocations
    {
        private static IModHelper Helper;

        public GameLocation Location = new GameLocation();
        public int CritterCount;
        public List<Critter> critterList;

        public class CritterLocation
        {
            public GameLocation location { get; set; } = new GameLocation();
            public string layerName { get; set; }
            public Vector2 tilePosition { get; set; } = new Vector2();
            public CritterEntry CritterEntry { get; set; } = new CritterEntry();
            public CritterLocation(GameLocation location, CritterEntry critter)
            {
                this.location = location;
                this.CritterEntry = critter;
                
            }
        }

        public static List<CritterLocation> CritterHomes = new List<CritterLocation>();

        

        public static void init(IModHelper helper)
        {
            Helper = helper;
            
        }
        public CritterLocations(GameLocation gameLocation)
        {
            Location = Game1.getLocationFromName(gameLocation.Name);
            new TileAction("disturbBug", DisturbBug).register();
            ButtonClick.UseToolButton.onClick(onDigBug);
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
        public void AddDiggableCritterToLocation(CritterEntry critter, Vector2 tilePosition, string layerName)
        {
            TerrainFeature terrainFeature = this.Location.terrainFeatures[tilePosition];
            CritterLocation critterLocation = new CritterLocation(Location, critter) { layerName = layerName, tilePosition = tilePosition };
            Log.info($"Registering critterLocation {critterLocation.location} {critterLocation.layerName} {critterLocation.tilePosition} {critterLocation.CritterEntry}");
            CritterHomes.AddOrReplace(critterLocation);
            Location.setTileProperty((int)tilePosition.X, (int)tilePosition.Y, "Back", "Diggable", "T");
            //ButtonClick.ActionButton.onClick(tilePosition, this.Location, DisturbBug);

        }
        public void AddDisturbableCritterToTerrainFeature(CritterEntry critter, Vector2 tilePosition, string layerName)
        {
            TerrainFeature terrainFeature = this.Location.terrainFeatures[tilePosition];
            CritterLocation critterLocation = new CritterLocation(Location, critter) { layerName = layerName, tilePosition = tilePosition };
            Log.info($"Registering critterLocation {critterLocation.location} {critterLocation.layerName} {critterLocation.tilePosition} {critterLocation.CritterEntry}");

            CritterHomes.AddOrReplace(critterLocation);
            this.Location.Map.addTouchAction(tilePosition, "disturbBug", "");

            //this.Location.isTileLocationTotallyClearAndPlaceable(tilePosition)
        }
        public bool DisturbBug(string action, GameLocation location, Vector2 tile, string layerName)
        {
            Log.debug($"{action} | {location} | {tile} | {layerName}");
            bool flag = false;
            CritterLocation locationEntry = new CritterLocation(location, (CritterEntry)null);
            locationEntry = CritterHomes.Find(c => c.layerName == layerName && c.location == location && c.tilePosition == tile);
            if (locationEntry != null)
            {
                Log.info("locationEntry found");
                CritterEntry critterEntry = new CritterEntry();
                critterEntry = CritterEntry.critters.Find(ce => ce.Value == locationEntry.CritterEntry).Value;
                Location.addCritter(critterEntry.makeCritter(tile * Game1.tileSize + new Vector2(Game1.tileSize / 2, Game1.tileSize / 2)));
                CritterHomes.Remove(locationEntry);
                flag = true;

            }
            return flag;
        }
        private void onDigBug(object sender, StardewModdingAPI.Events.ButtonPressedEventArgs args)
        {
            if (Game1.CurrentEvent != null)
                return;
            if (!args.Button.IsUseToolButton() || !(Game1.player.CurrentTool is StardewValley.Tools.Hoe))
                return;
            //Vector2 tile = args.Cursor.GrabTile;
            Vector2 tile = new Vector2((int)Game1.player.GetToolLocation(false).X / Game1.tileSize, (int)Game1.player.GetToolLocation(false).Y / Game1.tileSize);
            DisturbBug("", Location, tile, "Back");

        }


    }

}
