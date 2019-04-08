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
  
        internal List<BugModel> AllBugs = BugCatchingMod.AllBugs;
        internal List<NetModel> AllNets = BugCatchingMod.AllNets;

        public SObject sObject;
        public CustomObjectData data { get; set; }
        public PySync syncObject { get; set; }

        public NetModel netModel;
        public virtual Texture2D texture { get; private set; }

        private bool inUse;
        private static bool caughtBug;
        private static Critter Critter;

        public override string DisplayName { get => "Bug Net"; set => base.DisplayName = "Bug Net"; }
        public override string getDescription()
        {
            string text = netModel.Description;
            SpriteFont smallFont = Game1.smallFont;
            int width = Game1.tileSize * 4 + Game1.tileSize / 4;
            return Game1.parseText(text, smallFont, width);
        }

        public Texture2D loadTexture()
        {
            texture = BugCatchingMod._helper.Content.Load<Texture2D>(@"Assets/bugnet.png");
            return texture;
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
                data = CustomObjectData.collection.ContainsKey(netModel.FullId) ? CustomObjectData.collection[netModel.FullId] : new CustomObjectData(netModel.FullId, netModel.QuickItemDataString, this.loadTexture(), Color.White, netModel.TileIndex, true, typeof(BugNetTool));
            }
            if (sObject == null)
            {
                sObject = new SObject(data.sdvId, 1);
            }
            if (texture == null)
                loadTexture();

            Name = netModel.Name;
            description = netModel.Description;

            InitialParentTileIndex = 504;
            CurrentParentTileIndex = 504;
            IndexOfMenuItemView = 5;
            UpgradeLevel = 4;

            InstantUse = false;
            caughtBug = false;
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

        public override bool beginUsing(GameLocation location, int x, int y, StardewValley.Farmer who)
        {
            inUse = true;
            x = (int)who.GetToolLocation(false).X;
            y = (int)who.GetToolLocation(false).Y;
            Rectangle rectangle = new Rectangle(x - 32, y - 32, 64, 64);
            if (!caughtBug)
            {
                CritterLocations checkLocation = new CritterLocations(location);
                List<Critter> critters = new List<Critter>();
                critters = checkLocation.GetCritters();
                if (critters.Count > 0) 
                    foreach (Critter critter in critters)
                    {
                        if (critter.getBoundingBox(0, 0).Intersects(rectangle))
                        {
                            //Game1.addHUDMessage(new HUDMessage(critter.GetType().ToString(), 3));
                            Critter = critter;
                            caughtBug = true;
                            break;
                        }
                    }
            }
           

            return base.beginUsing(location, x, y, who);
        }

        public override void DoFunction(GameLocation location, int x, int y, int power, StardewValley.Farmer who)
        { 
            who.Stamina -= (float)(2 * power) - (float)who.FarmingLevel * 0.1f;
            power = who.toolPower;
            who.stopJittering();

            if ( caughtBug && Critter != null)
            {
                getBugFromNet(location, who); 
            }
            
           
        }

        public static void getBugFromNet(GameLocation location, StardewValley.Farmer who)
        {
            Bug BugInNet = BugApi.getBugFromCritterType(Critter);

            CritterLocations critterLocations = new CritterLocations(location);
            critterLocations.RemoveThisCritter(Critter);
            who.addItemByMenuIfNecessary((Item) BugInNet.getOne());

            who.AddCustomSkillExperience(BugCatchingMod.skill, BugInNet.bugModel.Price);
            Log.info("player experience: " + Game1.player.GetCustomSkillExperience(BugCatchingMod.skill).ToString());
            Critter = (Critter) null;
            caughtBug = false;
        }

       
        public override void endUsing(GameLocation location, Farmer who)
        {
            base.endUsing(location, who);
        }

        public override bool onRelease(GameLocation location, int x, int y, StardewValley.Farmer who)
        {
            inUse = false;
            return base.onRelease(location, x, y, who);
        }
       

        public override void leftClick(Farmer who)
        {
            base.leftClick(who);
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
            //CustomObjectData data = new CustomObjectData();
            //data = CustomObjectData.collection.Find(o => o.Key == netModel.FullId).Value;
            //Vector2 tileLoc = additionalSaveData["tileLocation"].Split(',').toList(i => i.toInt()).toVector<Vector2>();
            return new BugNetTool(netModel);
        }

    }
}