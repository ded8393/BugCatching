using System;
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
            CritterLocations.init(_helper);

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
                var net = new BugNetTool(netModel);
                CustomObjectData.newBigObject(netModel.FullId, net.loadTexture(), Color.White, netModel.Name, netModel.Description, netModel.TileIndex, netModel.Name, false, 0, false, false, "Crafting -9", 0, -300, new CraftingData(netModel.Name, netModel.Recipe), typeof(BugNetTool));
                Log.info($"adding {netModel.FullId} with recipe {netModel.Recipe}");

                //CustomObjectData.newObject($"{netModel.FullId}.Tool", new BugNetTool(netModel).loadTexture(), Color.White, netModel.Name, netModel.Description, netModel.TileIndex, "", "Net", 1, -300, craftingData: new CraftingData($"{netModel.FullId}.Tool", netModel.Recipe), customType: typeof(BugNetTool));
            }

            //_helper.Data.WriteJsonFile("data\\bugs.json", AssetData);
            Helper.Events.GameLoop.UpdateTicked -= LoadCritters;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="args">The event data.</param>
        private void onLocationChanged(object sender, StardewModdingAPI.Events.WarpedEventArgs args)
        {
            if (Game1.CurrentEvent != null)
                return;
            var CritterLocations = new CritterLocations(args.NewLocation);
            foreach (var entry in CritterEntry.critters)
            {
                var spawnTiles = entry.Value.attemptSpawn(args.NewLocation);
                if (spawnTiles.Count != 0)
                {
                    foreach (var tile in spawnTiles)
                    {
                        if (tile == null)
                            continue;
                        Monitor.Log($"Adding {entry.Value.BugModel.Name} at location {tile}");
                        if (entry.Value.BugModel.Classification == "Digger")
                            CritterLocations.AddDiggableCritterToLocation(entry.Value, tile, "Back");
                        else
                            args.NewLocation.addCritter(entry.Value.makeCritter(tile));
                    }
                    
                }
                
            }
        }
        private void catchBugDebris(object sender, DebrisListChangedEventArgs args)
        {
            if (!args.IsCurrentLocation)
                return;
            foreach (Debris debris in args.Added)
                if (debris.item.getCategoryName() == "Bug")
                    args.Location.debris.Remove(debris);
        }

    }
}