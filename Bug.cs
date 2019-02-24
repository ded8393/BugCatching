using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewValley.Objects;
using SObject = StardewValley.Object;
using Critter = StardewValley.BellsAndWhistles.Critter;

using PyTK.CustomElementHandler;
using PyTK.Extensions;
using PyTK.Types;

using StardewModdingAPI;

namespace BugCatching
{
    public class Bug : SObject, ICustomObject, ISyncableElement
    {
        internal static IModHelper Helper = BugCatchingMod._helper;
        internal IMonitor Monitor = BugCatchingMod._monitor;
        internal List<BugModel> AllBugs = BugCatchingMod.AllBugs;

        public SObject sObject;
        public CustomObjectData data { get; set; }
        public PySync syncObject { get; set; }

        public BugModel bugModel;

        public virtual Texture2D Texture { get; private set; }
        public virtual Rectangle sourceRectangle => Game1.getSourceRectForStandardTileSheet(Texture, bugModel.SpriteData.TileIndex, bugModel.SpriteData.FrameWidth, bugModel.SpriteData.FrameHeight);

        private Rectangle tilesize = new Rectangle(0, 0, 16, 16);
        private int tileIndex;
        private string id;
        private bool isPlain = false;

        public Bug()
        {
            syncObject = new PySync(this);
            syncObject.init(); 
        }

        public Bug(CustomObjectData data)
        {
            sObject = new SObject(data.sdvId, 1);
            this.data = data;
            build(AllBugs.Find(b => b.FullId == data.id));

        }


        public Bug(CustomObjectData data, Vector2 tileLocation, int stack)
        {
            sObject = new SObject(tileLocation, data.sdvId, stack);
            this.data = data;
            build(AllBugs.Find(b => b.FullId == data.id));
        }

        public Bug(CritterEntry data)
        {  
            bugModel = data.BugModel;
            build(bugModel);
        }

        public Bug(BugModel bugModel)
        {
            build(bugModel);
        }

        public override string getCategoryName()
        {
            return "Bug";
        }

        public virtual string getCatName(int cat)
        {
            return "Bug";
        }

        public override Color getCategoryColor()
        {
            return Color.DarkOrchid;
        }
        
        public override string DisplayName { get => name; set => base.DisplayName = value; }

        public override string getDescription()
        {
            return Game1.parseText(bugModel.Description, Game1.smallFont, Game1.tileSize * 4 + Game1.tileSize / 4);
        }

        public virtual void build(BugModel bugModel)
        {
            Monitor.Log("building bug" + bugModel.Name);
            this.bugModel = bugModel;
            if (data == null)
            {
                data = CustomObjectData.collection.ContainsKey(bugModel.FullId) ? CustomObjectData.collection[bugModel.FullId] : new CustomObjectData(bugModel.FullId, bugModel.QuickItemDataString, bugModel.SpriteData.getTexture(), Color.White, bugModel.SpriteData.TileIndex, true, typeof(Bug));
            }
            
            if (sObject == null)
            {
                sObject = new SObject(data.sdvId, 1);
            }
            if (Texture == null)
                Texture = bugModel.SpriteData.getTexture(Helper);

            id = bugModel.FullId;
            if (id.Contains("Plain."))
                isPlain = true;
            tileIndex = bugModel.SpriteData.TileIndex;
            bigCraftable.Value = false;
            type.Value = "Bug";
            ParentSheetIndex = data.sdvId;
            price.Value = bugModel.Price;

            tilesize = new Rectangle(0, 0, 16, 16);
            boundingBox.Value = new Rectangle(0, 0, tilesize.Width, tilesize.Height);
            name = bugModel.Name;
            if (syncObject == null)
            {
                syncObject = new PySync(this);
                syncObject.init();
            }
        }
      
        public override Item getOne()
        {
            return new Bug(data) { TileLocation = Vector2.Zero, name = name, Price = price, Quality = quality };
        }
       
        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, bool drawStackNumber, Color color, bool drawShadow)
        {
            if (isPlain)
                spriteBatch.Draw(Texture, location + new Vector2((Game1.tileSize), (Game1.tileSize)), sourceRectangle, Color.White * transparency, 0.0f, new Vector2(16f, 16f), Game1.pixelZoom, SpriteEffects.None, layerDepth);
            else
                spriteBatch.Draw(Texture, location + new Vector2((Game1.tileSize / 2 ), (Game1.tileSize / 2)), sourceRectangle, Color.White * transparency, 0.0f, new Vector2(16f,16f), Game1.pixelZoom * 0.25f, SpriteEffects.None, layerDepth);

            if (drawStackNumber && maximumStackSize() > 1 && (scaleSize > 0.3 && Stack != int.MaxValue) && Stack > 1)
                Utility.drawTinyDigits(stack, spriteBatch, location + new Vector2((Game1.tileSize - Utility.getWidthOfTinyDigitString(stack, 3f * scaleSize)) + 3f * scaleSize, (float)(Game1.tileSize - 18.0 * scaleSize + 2.0)), 3f * scaleSize, 1f, Color.White);
        }
        public override void drawWhenHeld(SpriteBatch spriteBatch, Vector2 objectPosition, Farmer f)
        {
            //todo: could move this down a little more
            if (isPlain)
                spriteBatch.Draw(Texture, objectPosition + new Vector2((Game1.tileSize / 4), (Game1.tileSize / 4)), sourceRectangle, Color.White, 0.0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, Math.Max(0.0f, (f.getStandingY() + 2) / 10000f));
            else
                spriteBatch.Draw(Texture, objectPosition + new Vector2((Game1.tileSize / 4), (Game1.tileSize / 4)) , sourceRectangle, Color.White, 0.0f, Vector2.Zero, Game1.pixelZoom * 0.25f, SpriteEffects.None, Math.Max(0.0f, (f.getStandingY() + 2) / 10000f));
        }
        public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1)
        {
            Monitor.Log("drawing");
            Rectangle tilesize = data.bigCraftable ? new Rectangle(0, 0, 16, 32) : new Rectangle(0, 0, 16, 16);
            Vector2 vector2 =  new Vector2(0.25f, 0.25f) * Game1.pixelZoom;
            Vector2 local = Game1.GlobalToLocal(Game1.viewport, new Vector2(((x / 64) * Game1.tileSize), ((y / 64) * Game1.tileSize - Game1.tileSize)));
            var r = sourceRectangle;
            Rectangle destinationRectangle = new Rectangle((int)(local.X - vector2.X / 2.0) + (shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0), (int)(local.Y - vector2.Y / 2.0) + (shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0), (int)((tilesize.Width * 4) + (double)vector2.X), (int)((tilesize.Height * 4) + vector2.Y / 2.0));
            var newSR = new Rectangle?(new Rectangle((int)(r.X * 0.25f), (int)(r.Y * 0.25f), (int)(r.Width * 0.25f), (int)(r.Height * 0.25f)));
            spriteBatch.Draw(Texture, destinationRectangle, newSR, Color.White * alpha, 0.0f, Vector2.Zero, SpriteEffects.None, (float)(Math.Max(0.0f, ((y + 1) * Game1.tileSize - Game1.pixelZoom * 6) / 10000f) + x * 9.99999974737875E-06));
        }
        public override void draw(SpriteBatch spriteBatch, int xNonTile, int yNonTile, float layerDepth, float alpha = 1)
        {
            draw(spriteBatch, xNonTile, yNonTile, alpha);
        }

        //public virtual void actionWhenBeingHeld(Farmer who)
        //{
        //}

        //public virtual void actionWhenStopBeingHeld(Farmer who)
        //{
        //}
        public override void drawPlacementBounds(SpriteBatch spriteBatch, GameLocation location){ }

        public override bool canBeTrashed()
         {
                return true;
         }
        public override bool canBeGivenAsGift()
        {
            return true;
        }
        public override bool isPlaceable()
        {
            return true;
        }
        public override bool canBePlacedHere(GameLocation l, Vector2 tile)
        {
            return true;
        }
        public override bool placementAction(GameLocation location, int x, int y, Farmer who = null)
        {
            CritterEntry livingBug = new CritterEntry();

            if (isPlain)
            {
                BugModel bugModel = new BugModel();
                bugModel = BugApi.findOrCreateBugModelFromId(this.bugModel.FullId);
                livingBug.BugModel = bugModel;
            }
            else
            {
                livingBug = CritterEntry.critters.Find(ce => ce.Value.BugModel.FullId == this.bugModel.FullId).Value;
            }
               
            location.addCritter(livingBug.makeCritter(new Vector2(x, y)));
            return true;
        }

        public  ICustomObject recreate(Dictionary<string, string> additionalSaveData, object replacement)
         {
            Log.info("recreating: " + additionalSaveData["bugId"]);
            BugModel bugModel = BugApi.findOrCreateBugModelFromId(additionalSaveData["bugId"]);
            return new Bug(bugModel);
         }

        public object getReplacement()
        {
            Chest r = new Chest(true);
            r.items.Add(heldObject);
            return r;
        }

        public Dictionary<string, string> getAdditionalSaveData()
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("bugId", bugModel.FullId);
            data.Add("stack", stack.ToString());

            return data;
        }

        public void rebuild(Dictionary<string, string> additionalSaveData, object replacement)
        {
            Log.info("rebuilding bug: " + additionalSaveData["bugId"]);
            BugModel bugModel = BugApi.findOrCreateBugModelFromId(additionalSaveData["bugId"]);
            build(bugModel);
            stack.Value = int.Parse(additionalSaveData["stack"]);
        }

        public Dictionary<string, string> getSyncData()
        {
            return getAdditionalSaveData();
        }

        public void sync(Dictionary<string, string> syncData)
        {
            rebuild(syncData, heldObject);
        }
    }
   
}
