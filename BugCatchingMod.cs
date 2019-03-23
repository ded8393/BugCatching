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
        public static BugCatchingSkill skill;

        internal static DataInjector DataInjector;

        public override void Entry(IModHelper helper)
        {
            instance = this;
            _helper = helper;
            _monitor = Monitor;
            ModId = _helper.ModRegistry.ModID;


            BugNetTool bugnet = new BugNetTool();
            new InventoryItem(bugnet, 100).addToNPCShop("Pierre");
            // todo change to .Tool v
            CustomObjectData.newObject(ModId + "Tool", BugNetTool.texture, Color.White, "Bug Net", "Wonder what I can do with this?", 5, customType: typeof(BugNetTool));


            _helper.Events.GameLoop.UpdateTicked += LoadCritters;
            BugApi.init(_helper);
            CritterLocations.init(_helper);
            //PyLua.registerType(typeof(BugNetTool), registerAssembly: true);
            //PyLua.registerType(typeof(Bug), registerAssembly: true);

            _helper.Events.Player.Warped += onLocationChanged;

            DataInjector = new DataInjector(_helper);
            Game1.ResetToolSpriteSheet();
            Skills.RegisterSkill(skill = new BugCatchingSkill());

        }
        public void LoadCritters(object sender, StardewModdingAPI.Events.UpdateTickedEventArgs e)
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
                bugModel.ParentSheetIndex = Id;
                AllBugs.AddOrReplace(bugModel);
                CustomObjectData.newObject(bugModel.FullId,  bugModel.SpriteData.getTexture(), Color.White, bugModel.Name, bugModel.Description, bugModel.SpriteData.TileIndex, price:bugModel.Price, customType: typeof(Bug));
                //AssetData[bugData.sdvId] = bugModel.QuickItemDataString;
                Monitor.Log("Added: " + bugModel.Name + " id " + bugModel.FullId.ToString());
                Id--;
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
                var spawnLocs = entry.Value.attemptSpawn(args.NewLocation);
                if (spawnLocs.Count != 0)
                {
                    foreach (var spawnLoc in spawnLocs)
                    {
                        Monitor.Log(entry.Value.BugModel.Name + " at location " + spawnLoc.ToString());
                        // this.map.GetLayer("Back").Tiles[xLocation, yLocation].Properties.Add("Treasure", new PropertyValue("Object " + (object) "bug"));
                        //if (entry.Value.Behavior.Classification == "Digger")
                        //    CritterLocations.AddDiggableCritterToLocation(entry.Value, spawnLoc, "Back");
                        //else
                            args.NewLocation.addCritter(entry.Value.makeCritter(args.NewLocation, spawnLoc));
                    }
                    
                }
                
            }
        }

    }
}