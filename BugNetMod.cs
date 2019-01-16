using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

namespace BugNet
{
    /// <summary>The mod entry point.</summary>
            
    public class BugNetMod : Mod
    {
        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public static IModHelper _helper;
        public static IMonitor _monitor;
        public static BugNetData bugNetData;
        public static List<BugModel> AllBugs = new List<BugModel>();
        public static List<CritterEntry> AllCritters = new List<CritterEntry>();
       // public static Dictionary<int, string> AssetData = new Dictionary<int, string>();
        public static string ModId;
        public static Mod instance;
        public override void Entry(IModHelper helper)
        {
            instance = this;
            _helper = helper;
            _monitor = Monitor;
            ModId = _helper.ModRegistry.ModID;
            BugNetTool bugnet = new BugNetTool();
            new InventoryItem(bugnet, 100).addToNPCShop("Pierre");
            CustomObjectData.newObject(ModId + "Tool", BugNetTool.texture, Color.White, "Bug Net", "Wonder what I can do with this?", 0, customType: typeof(BugNetTool));
            Helper.Events.GameLoop.UpdateTicked += LoadCritters;
            CritterLocations.init(Helper);
            //PyLua.registerType(typeof(BugNetTool), registerAssembly: true);
            //PyLua.registerType(typeof(Bug), registerAssembly: true);

            Helper.Events.Player.Warped += onLocationChanged;
            

        }
        //public bool CanEdit<T>(IAssetInfo asset)
        //{
        //    return asset.AssetNameEquals("Data\\ObjectInformation");
        //}
        //public void Edit<T>(IAssetData asset)
        //{
        //    if (asset.AssetNameEquals("Data\\ObjectInformation"))
        //    {
        //        var data = asset.AsDictionary<int, string>().Data;
        //        Dictionary<int, string> BugsData = _helper.Data.ReadJsonFile<Dictionary<int, string>>("data\\bugs.json");
        //        foreach(var bug in BugsData)
        //        {
        //            data[bug.Key] = bug.Value;
        //        }

        //    }
        //}

        public void LoadCritters(object sender, StardewModdingAPI.Events.UpdateTickedEventArgs e)
        {
            BugNetData data = _helper.Data.ReadJsonFile<BugNetData>("Assets/critters.json");
            AllCritters = new List<CritterEntry>();
           //Dictionary<int, string> AssetData = new Dictionary<int, string>();
            foreach (CritterEntry critter in data.AllCritters)
            {
                AllCritters.AddOrReplace(critter);
                CritterEntry.Register(critter);
                var bugModel = new BugModel();
                bugModel = critter.BugModel;
                AllBugs.AddOrReplace(bugModel);
                CustomObjectData.newObject(bugModel.FullId,  bugModel.getTexture(), Color.White, bugModel.Name, bugModel.Description, bugModel.TileIndex,price:bugModel.Price, customType: typeof(Bug));
                //AssetData[bugData.sdvId] = bugModel.QuickItemDataString;
                Monitor.Log("Added: " + bugModel.Name + " id " + bugModel.FullId.ToString());
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

            foreach (var entry in CritterEntry.critters)
            {
                Monitor.Log(entry.ToString());
                var spawnLoc = entry.Value.attemptSpawn(args.NewLocation);
                if (spawnLoc != null)
                {
                    args.NewLocation.addCritter(entry.Value.makeCritter(spawnLoc.Value));
                }
                
            }
        }

    }
}