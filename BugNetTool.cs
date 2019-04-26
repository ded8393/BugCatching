using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using StardewValley;
using StardewValley.Tools;
using StardewValley.Objects;

using SObject = StardewValley.Object;
using Critter = StardewValley.BellsAndWhistles.Critter;

using PyTK.CustomElementHandler;
using PyTK.Extensions;
using SpaceCore;

using StardewModdingAPI;

namespace BugCatching
{
    public class BugNetTool : Hoe, ISaveElement, ICustomObject
    {
        internal IMonitor Monitor = BugCatchingMod._monitor;
        //move refs to BugApi
        internal List<BugModel> AllBugs = BugCatchingMod.AllBugs;
        internal List<NetModel> AllNets = BugCatchingMod.AllNets;

        public SObject sObject;
        public CustomObjectData data { get; set; }
        public PySync syncObject { get; set; }

        public NetModel netModel;
        public virtual Texture2D texture { get; private set; }

        private bool inUse;
        private bool caughtBug;
        private static Critter CaughtCritter;
        private static Bug BugInNet;

        public override string getDescription()
        {
            string text = netModel.Description;
            SpriteFont smallFont = Game1.smallFont;
            int width = Game1.tileSize * 4 + Game1.tileSize / 4;
            return Game1.parseText(text, smallFont, width);
        }
       
        public BugNetTool()
             : base()
        {
        }
        public BugNetTool(NetModel netModel)
           :this()
        {
            build(netModel);
        }

        public BugNetTool(CustomObjectData data)
            :this()
        {
            sObject = new SObject(data.sdvId, 1);
            this.data = data;
            build(AllNets.Find(n => n.FullId == data.id));
        }
        public BugNetTool(CustomObjectData data, Vector2 tileLocation)
            :this()
        {
            sObject = new SObject(tileLocation, data.sdvId);
            this.data = data;
            build(AllNets.Find(n => n.FullId == data.id));
        }

        private void build(NetModel netModel)
        {
            this.netModel = netModel;
            if (data == null)
            {
                data = CustomObjectData.collection.ContainsKey(netModel.FullId) ? CustomObjectData.collection[netModel.FullId] : new CustomObjectData(netModel.FullId, netModel.QuickItemDataString, netModel.getTexture(), Color.White, netModel.TileIndex, true, typeof(BugNetTool));
            }
            if (sObject == null)
            {
                sObject = new SObject(data.sdvId, 1);
            }
            if (texture == null)
                texture = netModel.getTexture();

            Name = netModel.Name;
            DisplayName = Name;
            description = netModel.Description;

            InitialParentTileIndex = 504;
            CurrentParentTileIndex = 504;
            IndexOfMenuItemView = 5;
            UpgradeLevel = 1;

            InstantUse = false;
            inUse = false;
        }

        public override void setNewTileIndexForUpgradeLevel()
        {
        }

        public override Item getOne()
        {
            return new BugNetTool(netModel);
        }

        public override bool canBeTrashed()
        {
            return true;
        }
        public override bool doesShowTileLocationMarker()
        {
            return false;
        }

        public override bool actionWhenPurchased()
        {
            return true;
        }

        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, bool drawStackNumber, Color color, bool drawShadow)
        {
            spriteBatch.Draw(texture, location + new Vector2((Game1.tileSize / 2), 0), new Rectangle?(Game1.getSquareSourceRectForNonStandardTileSheet(texture, 16, 32, IndexOfMenuItemView)), color * transparency, 0.0f, new Vector2(8f, 16f), 4f * scaleSize, SpriteEffects.None, layerDepth);
            StardewValley.Farmer f = Game1.player;
            this.Update(f.FacingDirection, 0, f);
        }

        public override void draw(SpriteBatch b)
        {
            base.draw(b);
        }
        //public Rectangle getNetZone(Farmer who)
        //{

        //}

        public override bool beginUsing(GameLocation location, int x, int y, StardewValley.Farmer who)
        {
            inUse = true;
            Log.info("beginging hte use");
            Rectangle rectangle = new Rectangle((int)who.GetToolLocation(false).X, (int)who.GetToolLocation(false).Y, 64, 64);

            CritterLocations checkLocation = new CritterLocations(location);
            List<Critter> critters = new List<Critter>();
            critters = checkLocation.getAllActiveCritters();

            if (!caughtBug)
                if (critters.Count > 0)
                    foreach (Critter critter in critters)
                        if (checkCatch(critter, rectangle))
                            break; 


            return base.beginUsing(location, x, y, who);
        }

        public bool checkCatch(Critter critter, Rectangle catchZone)
        {
            caughtBug = false;
            Log.info($"checking the critter {critter.GetHashCode().ToString()}");
            if (critter.getBoundingBox(0, 0).Intersects(catchZone))
            {
                BugModel bug = BugApi.createBugModelFromCritter(critter);
                if (bug.Rarity < netModel.maxRarity)
                {
                    CaughtCritter = critter;
                    caughtBug = true;
                    Log.info($"Caught a bug {bug.Name}");
                }
                else
                {
                    Game1.addHUDMessage(new HUDMessage($"The {bug.Name} escaped your {this.Name}"));
                    //check for bugHasItem
                }
  
            }
            return caughtBug;
        }

        public override void DoFunction(GameLocation location, int x, int y, int power, StardewValley.Farmer who)
        { 
            who.Stamina -= (float)(2 * power) - (float)who.GetCustomSkillLevel(BugCatchingMod.skill) * 0.1f;

            Log.info($"Doing a functions");
            if (caughtBug & CaughtCritter != null)
            {
                Log.info("yes the critter is gud");
                BugInNet = BugApi.getBugFromCritterType(CaughtCritter);
                getBugFromNet(location, who); 
                
            }
            
           
        }

        public static void getBugFromNet(GameLocation location, StardewValley.Farmer who)
        {
            Log.info($"Now the bug comes from the net");
            CritterLocations critterLocations = new CritterLocations(location);
            critterLocations.removeThisCritter(CaughtCritter);
            who.addItemByMenuIfNecessary((Item) BugInNet.getOne());

            who.AddCustomSkillExperience(BugCatchingMod.skill, BugInNet.bugModel.Price);
            Log.info("player experience: " + Game1.player.GetCustomSkillExperience(BugCatchingMod.skill).ToString());
            CaughtCritter = (CustomCritter) null;
            
        }

        public override void leftClick(Farmer who){
            Log.info("the left click");
            base.leftClick(who); }       
        public override void endUsing(GameLocation location, Farmer who)
        {
            inUse = false;
            Log.info("end using");
            base.endUsing(location, who);
        }

        public override bool onRelease(GameLocation location, int x, int y, StardewValley.Farmer who)
        {
            Log.info("releasing");
            return base.onRelease(location, x, y, who);
        }




        public Dictionary<string, string> getAdditionalSaveData()
        {
            Dictionary<string, string> savedata = new Dictionary<string, string>();
            savedata.Add("id", netModel.FullId);
            savedata.Add("tileLocation", sObject.TileLocation != null ? sObject.TileLocation.X + "," + sObject.TileLocation.Y : "0,0");
            return savedata;
        }

        public dynamic getReplacement()
        {
            Chest r = new Chest(true);
            r.items.Add(sObject.getOne());
            r.TileLocation = sObject.TileLocation;
            return r;
        }

        public void rebuild(Dictionary<string, string> additionalSaveData, object replacement)
        {
            Log.info($"rebuilding : {additionalSaveData["id"]}");
            NetModel netModel = AllNets.Find(n => n.FullId == additionalSaveData["id"]);
            build(netModel);
            sObject.TileLocation = additionalSaveData["tileLocation"].Split(',').toList(i => i.toInt()).toVector<Vector2>();

        }
        public ICustomObject recreate(Dictionary<string, string> additionalSaveData, object replacement)
        {
            NetModel netModel = AllNets.Find(n => n.FullId == additionalSaveData["id"]);
            return new BugNetTool(netModel);
        }

    }
}