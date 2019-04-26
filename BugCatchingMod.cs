using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;

using PyTK;
using PyTK.Types;
using PyTK.Extensions;
using PyTK.CustomElementHandler;
using PyTK.Lua;

using StardewValley;
using Critter = StardewValley.BellsAndWhistles.Critter;
using SpaceCore;

namespace BugCatching
{
    /// <summary>The mod entry point.</summary>
            
    public class BugCatchingMod : Mod
    {
        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public static IModHelper _helper;
        public static IMonitor _monitor;
        public static string ModId;
        public static Mod instance;


        public static BugNetData bugNetData;
        public static List<BugModel> AllBugs = new List<BugModel>();
        public static List<CritterEntry> AllCritters = new List<CritterEntry>();
        public static List<NetModel> AllNets = new List<NetModel>();
        public static List<CritterLocations> AllCritterLocations = new List<CritterLocations>();
        public static List<CritterEntry> SeasonalCritters = new List<CritterEntry>();
        public static BugCatchingSkill skill;

        internal static DataInjector DataInjector;

        public override void Entry(IModHelper helper)
        {
            instance = this;
            _helper = helper;
            _monitor = Monitor;
            ModId = _helper.ModRegistry.ModID;

            _helper.Events.GameLoop.UpdateTicked += LoadCritters;
            BugApi.init(_helper);

            new TileAction("disturbBug", DisturbBug).register();
            ButtonClick.UseToolButton.onClick(onDigBug);

            _helper.Events.GameLoop.DayStarted += createCritterLocationList;
            //_helper.Events.World.LocationListChanged += updateCritterLocationsList;
            _helper.Events.Player.Warped += onLocationChanged;
            _helper.Events.World.DebrisListChanged += catchBugDebris;
            DataInjector = new DataInjector(_helper);
            Game1.ResetToolSpriteSheet();
            Skills.RegisterSkill(skill = new BugCatchingSkill());

        }
        public void LoadCritters(object sender, UpdateTickedEventArgs e)
        {
            BugNetData data = _helper.Data.ReadJsonFile<BugNetData>("Assets/critters.json");
            int Id = -666;
            AllCritters = new List<CritterEntry>();
            Dictionary<int, string> AssetData = new Dictionary<int, string>();
            foreach (CritterEntry critter in data.AllCritters)
            {
                AllCritters.AddOrReplace(critter);
                CritterEntry.Register(critter);
                var bugModel = new BugModel();
                bugModel = critter.BugModel;
                //is this V necessary ?
                bugModel.ParentSheetIndex = Id;
                AllBugs.AddOrReplace(bugModel);
                CustomObjectData.newObject(bugModel.FullId,  bugModel.SpriteData.getTexture(), Color.White, bugModel.Name, bugModel.Description, bugModel.SpriteData.TileIndex, price:bugModel.Price, customType: typeof(Bug));
                //AssetData[bugData.sdvId] = bugModel.QuickItemDataString;
                Monitor.Log("Added: " + bugModel.Name + " id " + bugModel.FullId.ToString());
                Id--;
            }

            foreach(NetModel netModel in data.AllNets)
            {
                
                AllNets.AddOrReplace(netModel);
                //net.ParentSheetIndex = Id;
 
                CustomObjectData.newBigObject(netModel.FullId, netModel.getTexture(), Color.White, netModel.Name, netModel.Description, netModel.TileIndex, netModel.Name, false, 0, false, false, "Crafting -9", 0, -300, new CraftingData(netModel.Name, netModel.Recipe), typeof(BugNetTool));
                Log.info($"adding {netModel.FullId} with recipe {netModel.Recipe}");

                //CustomObjectData.newObject($"{netModel.FullId}.Tool", new BugNetTool(netModel).loadTexture(), Color.White, netModel.Name, netModel.Description, netModel.TileIndex, "", "Net", 1, -300, craftingData: new CraftingData($"{netModel.FullId}.Tool", netModel.Recipe), customType: typeof(BugNetTool));
            }

            //_helper.Data.WriteJsonFile("data\\bugs.json", AssetData);
            _helper.Events.GameLoop.UpdateTicked -= LoadCritters;
        }


        /*********
        ** Private methods
        *********/
        private void createCritterLocationList(object sender, StardewModdingAPI.Events.DayStartedEventArgs args)
        {

            Log.debug("in create critter day start event");
            SeasonalCritters = AllCritters.Where(c => c.SpawnConditions.Seasons.Contains(Game1.currentSeason)).ToList();
            CritterLocations Farm = new CritterLocations(Game1.getFarm());
            Farm.buildAllPossibleHomes();
            Farm.balanceSpawning();
            AllCritterLocations.AddOrReplace(Farm);
            //Helper.Events.GameLoop.DayStarted -= createCritterLocationList;
        }
        private void updateCritterLocationsList(object sender, LocationListChangedEventArgs args)
        {
            if (args.Added.Count() > 0)
            {
                foreach (GameLocation location in args.Added.ToList())
                {
                    updateCritterLocations(location);
                }

            }
            if (args.Removed.Count() > 0)
                AllCritterLocations.RemoveAll(l => args.Removed.Contains(l.Location));

           //Helper.Events.World.LocationListChanged -= updateCritterLocationsList;
        }
        internal void updateCritterLocations(GameLocation location)
        {

            if (location.IsOutdoors)
            {
                CritterLocations thisLocation = new CritterLocations(location);
                thisLocation.buildAllPossibleHomes();
                thisLocation.balanceSpawning();
                AllCritterLocations.AddOrReplace(thisLocation);
            }
        }
        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="args">The event data.</param>
        /// 
        private void onLocationChanged(object sender, StardewModdingAPI.Events.WarpedEventArgs args)
        {
            Log.info($"Location changed to {args.NewLocation}");
            try
            {
                CritterLocations CritterLocator = AllCritterLocations.Where(l => l.Location == Game1.currentLocation).SingleOrDefault();
                CritterLocator.spawnCritters();
            }
            catch 
            {
                Log.debug("CritterLocator failed, creating new CritterLocations");
                CritterLocations CritterLocator = new CritterLocations(args.NewLocation);
                CritterLocator.buildAllPossibleHomes();
                CritterLocator.balanceSpawning();
                CritterLocator.spawnCritters();
                AllCritterLocations.AddOrReplace(CritterLocator);
            }

           // Helper.Events.Player.Warped -= onLocationChanged; 
        }
        private void onDigBug(object sender, StardewModdingAPI.Events.ButtonPressedEventArgs args)
        {
            if (Game1.CurrentEvent != null)
                return;
            if (!args.Button.IsUseToolButton() || !(Game1.player.CurrentTool is StardewValley.Tools.Hoe))
                return;

            Vector2 tile = new Vector2((int)Game1.player.GetToolLocation(false).X / Game1.tileSize + 0.5f, (int)Game1.player.GetToolLocation(false).Y / Game1.tileSize + 0.5f);
            DisturbBug("", Game1.currentLocation, tile, "Back");
        }

        public bool DisturbBug(string action, GameLocation location, Vector2 tile, string layerName)
        {
            Log.debug($"{action} | {location} | {tile} | {layerName}");
            bool flag = false;

            CritterLocations CritterLocator = AllCritterLocations.Where(l => l.Location == Game1.currentLocation).Single();
            CritterLocation critterLocation = CritterLocator.getCritterHome(layerName, tile);

            if (critterLocation != null)
            {
                CritterLocator.activateCritter(critterLocation);
                flag = true;
            }

            return flag;
        }
        private void catchBugDebris(object sender, DebrisListChangedEventArgs args)
        {
            if (!args.IsCurrentLocation)
                return;
            foreach (Debris debris in args.Added)
                if (debris.item!=null)
                    if (debris.item.getCategoryName() == "Bug")
                        args.Location.debris.Remove(debris);

        }

    }
}