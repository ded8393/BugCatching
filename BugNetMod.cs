using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;

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
        public static string ModId;
        public override void Entry(IModHelper helper)
        {
            _helper = helper;
            _monitor = Monitor;
            ModId = _helper.ModRegistry.ModID;
            BugNetTool bugnet = new BugNetTool();
            new InventoryItem(bugnet, 100).addToNPCShop("Pierre");
            CustomObjectData.newObject(ModId + "Tool", BugNetTool.texture, Color.White, "Bug Net", "Wonder what I can do with this?", 0, customType: typeof(BugNetTool));
            Helper.Events.GameLoop.UpdateTicked += LoadBugData;
            CritterLocations.init(Helper);
            PyLua.registerType(typeof(Bug), registerAssembly: true);

        }
        public void LoadBugData(object sender, StardewModdingAPI.Events.UpdateTickedEventArgs e)
        {
            BugNetData data = _helper.Data.ReadJsonFile<BugNetData>("Assets/data.json");
            AllBugs = new List<BugModel>();
            foreach(BugModel bugModel in data.AllBugs)
            {
                AllBugs.AddOrReplace(bugModel);
                Texture2D texture = bugModel.getTexture();
                var bugObj = new CustomObjectData(bugModel.FullId, bugModel.QuickItemDataString, texture, Color.White, bugModel.TileIndex, true, typeof(Bug));
                Monitor.Log("Added: " + bugModel.Name);
            }

            Helper.Events.GameLoop.UpdateTicked -= LoadBugData;
        }
        

        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;
            return;
            // print button presses to the console window
            //this.Monitor.Log($"{Game1.player.currentLocation.} pressed {e.Button}.");
        }
        
    }
}