using System;
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

using StardewModdingAPI;

namespace BugNet
{
    public class BugNetTool : Hoe, ISaveElement, ICustomObject
    {
        internal IMonitor Monitor = BugNetMod._monitor;

        internal List<BugModel> AllBugs = BugNetMod.AllBugs;
        internal static Texture2D texture;
        private bool inUse;
        private static bool caughtBug;
        private static Critter BugInNet;

        

        public Dictionary<string, string> getAdditionalSaveData()
        {
            Dictionary<string, string> savedata = new Dictionary<string, string>();
            savedata.Add("name", Name);
            return savedata;
        }

        public dynamic getReplacement()
        {
            //BugNetTool replacement = new BugNetTool();
            //return replacement;
            Chest replacement = new Chest(true);
            return replacement;
        }

        public void rebuild(Dictionary<string, string> additionalSaveData, object replacement)
        {
            build();
            Chest chest = (Chest)replacement;

        }

        public BugNetTool(CustomObjectData data)
            : this()
        {
        }

        public BugNetTool()
            : base()
        {
            build();
        }

        public override bool canBeTrashed()
        {
            return true;
        }

        internal static void loadTextures()
        {
            texture = BugNetMod._helper.Content.Load<Texture2D>(@"Assets/bugnet.png");
           
        }


        public override bool actionWhenPurchased()
        {
            return true;
        }

        public override Item getOne()
        {
            return new BugNetTool();
        }
        

        public override void setNewTileIndexForUpgradeLevel()
        {
        }

        public override string DisplayName { get => "Bug Net"; set => base.DisplayName = "Bug Net"; }

        private void build()
        {
            if (texture == null)
                loadTextures();

            Name = "Bug Net";
            description = "catch critters";

            InitialParentTileIndex = 77;
            CurrentParentTileIndex = 77;
            IndexOfMenuItemView = 0;
            UpgradeLevel = 4;

            InstantUse = false;
            caughtBug = false;
            inUse = false;
        }

        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, bool drawStackNumber, Color color, bool drawShadow)
        {
            spriteBatch.Draw(texture, location + new Vector2(32f, 32f), new Rectangle?(Game1.getSquareSourceRectForNonStandardTileSheet(texture, 16, 16, this.IndexOfMenuItemView)), color * transparency, 0.0f, new Vector2(8f, 8f), 4f * scaleSize, SpriteEffects.None, layerDepth);

            if (inUse)
            {
                StardewValley.Farmer f = Game1.player;
                Vector2 vector = f.getLocalPosition(Game1.viewport) + f.jitter + f.armOffset;
                int num = (int)vector.Y - ((Game1.tileSize * 5) / 2);
                spriteBatch.Draw(texture, new Vector2(vector.X, (float)num), new Rectangle?(Game1.getSquareSourceRectForNonStandardTileSheet(Game1.toolSpriteSheet, 16, 16, this.IndexOfMenuItemView)), color * transparency, 0.0f, new Vector2(8f, 8f), 4f * scaleSize, SpriteEffects.None, Math.Max(0f, (float)(f.getStandingY() + Game1.tileSize / 2) / 10000f));
            }

        }

        public override bool beginUsing(GameLocation location, int x, int y, StardewValley.Farmer who)
        {
            inUse = true;
            x = (int)who.GetToolLocation(false).X;
            y = (int)who.GetToolLocation(false).Y;
            Rectangle rectangle = new Rectangle(x - 32, y - 32, 64, 64);
            if (!caughtBug)
            {
                List<Critter> critters = CritterLocations.GetCrittersAtGameLocation(location);
                foreach (Critter critter in critters)
                {
                    if (critter.getBoundingBox(0, 0).Intersects(rectangle))
                    {
                        Game1.addHUDMessage(new HUDMessage("babied", 3));
                        BugInNet = critter;
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

            if ( caughtBug && BugInNet != null)
            {

                var bugName = BugInNet.GetType().ToString();
                

                string msg = "location: " + location.GetType().ToString() + ", critter: " + bugName;

                Game1.showRedMessage(msg);

                getBugFromNet(location, who);
                
                
            }
            
           
        }
        public static void getBugFromNet(GameLocation location, StardewValley.Farmer who)
        {
            Bug bug;
        
            if (BugInNet.GetType().ToString() == "BugNet.CustomCritter")
            {
                bug = new Bug((CustomCritter)BugInNet);
            } else
            {
                BugApi bugApi = new BugApi();
                bug = bugApi.getBugFromCritterType(BugInNet);

            }
        
            CritterLocations.RemoveCritterFromGameLocation(location, BugInNet);
            who.addItemByMenuIfNecessary((Item) bug.getOne());
            BugInNet = (Critter) null;
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
        public override string getDescription()
        {  
            string text = description;
            SpriteFont smallFont = Game1.smallFont;
            int width = Game1.tileSize * 4 + Game1.tileSize / 4;
            return Game1.parseText(text, smallFont, width);
        }

        public override void leftClick(Farmer who)
        {
            base.leftClick(who);
        }

        public ICustomObject recreate(Dictionary<string, string> additionalSaveData, object replacement)
        {
            return new BugNetTool();
        }

    }
}