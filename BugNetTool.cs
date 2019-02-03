using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using Netcode;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using StardewValley;
using StardewValley.Tools;
using StardewValley.Objects;
using SObject = StardewValley.Object;
using Critter = StardewValley.BellsAndWhistles.Critter;
using PyTK.CustomElementHandler;
using PyTK.Types;
using customObj = PyTK.CustomElementHandler.CustomObjectData;
using SpaceCore;

using StardewModdingAPI;

namespace BugCatching
{
    public class BugNetTool : Hoe, ISaveElement, ICustomObject
    {
        internal IMonitor Monitor = BugCatchingMod._monitor;

        internal List<BugModel> AllBugs = BugCatchingMod.AllBugs;
        internal static Texture2D texture;
        private bool inUse;
        private static bool caughtBug;
        private static Critter Critter;

        public override string DisplayName { get => "Bug Net"; set => base.DisplayName = "Bug Net"; }
        public override string getDescription()
        {
            string text = description;
            SpriteFont smallFont = Game1.smallFont;
            int width = Game1.tileSize * 4 + Game1.tileSize / 4;
            return Game1.parseText(text, smallFont, width);
        }

        internal static void loadTextures()
        {
            texture = BugCatchingMod._helper.Content.Load<Texture2D>(@"Assets/bugnet.png");
        }

        public BugNetTool()
            : base()
        {
            build();
        }

        public BugNetTool(CustomObjectData data) : this() { }

        private void build()
        {
            if (texture == null)
                loadTextures();

            Name = "Bug Net";
            description = "catch critters";

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
            return new BugNetTool();
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
            Game1.player.AddCustomSkillExperience(BugCatchingMod.skill, BugInNet.bugModel.Price);
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
            savedata.Add("name", Name);
            return savedata;
        }

        public dynamic getReplacement()
        {
            return new Chest(true);
        }

        public void rebuild(Dictionary<string, string> additionalSaveData, object replacement)
        {
            build();
        }
        public ICustomObject recreate(Dictionary<string, string> additionalSaveData, object replacement)
        {
            return new BugNetTool();
        }

    }
}