using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using SObject = StardewValley.Object;
using Critter = StardewValley.BellsAndWhistles.Critter;

using PyTK.CustomElementHandler;
using PyTK.Extensions;
using PyTK.Types;

using StardewModdingAPI;

namespace BugNet
{
    public class Bug : PySObject
    {
        internal IModHelper Helper = BugNetMod._helper;
        internal IMonitor Monitor = BugNetMod._monitor;
        internal List<BugModel> AllBugs = BugNetMod.AllBugs;

        public CritterEntry Critter;
        public BugModel bugModel;
        public virtual Texture2D Texture { get; private set; }
        public virtual Rectangle sourceRectangle => Game1.getSourceRectForStandardTileSheet(Texture, bugModel.TileIndex, bugModel.OriginalWidth, bugModel.OriginalWidth);

        private Rectangle tilesize = new Rectangle(0, 0, 16, 16);
        private int tileIndex;
        private string id;

        public Bug()
        {
            syncObject = new PySync(this);
            syncObject.init(); 
        }

        public Bug(CustomObjectData data)
               :base(data)
        {
            sObject = new SObject(data.sdvId, 1);
            this.data = data;
            checkData();
            build(AllBugs.Find(b => b.FullId == data.id)); 
           
        }

        public Bug(CustomObjectData data, Vector2 tileLocation, int stack)
        {
            sObject = new SObject(tileLocation, data.sdvId, stack);
            this.data = data;
            checkData();
            build(AllBugs.Find(b => b.FullId == data.id));
        }

        public Bug(CustomCritter critter)
        {
            Critter = critter.data;
            bugModel = Critter.BugModel;
            build(bugModel);
            checkData();
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
            return Color.LimeGreen;
        }
        
        public override string DisplayName { get => name; set => base.DisplayName = value; }

        public override string getDescription()
        {
            return Game1.parseText(bugModel.Description, Game1.smallFont, Game1.tileSize * 4 + Game1.tileSize / 4);
        }

        public virtual void build(BugModel bugModel)
        {
            Monitor.Log("building bug");
            if (syncObject == null)
            {
                syncObject = new PySync(this);
                syncObject.init();
            }

            this.bugModel = bugModel;
            if (data == null)
            {
                data = CustomObjectData.collection.ContainsKey(bugModel.FullId) ? CustomObjectData.collection[bugModel.FullId] : new CustomObjectData(bugModel.FullId, bugModel.QuickItemDataString, bugModel.getTexture(), Color.White, bugModel.TileIndex, true, typeof(Bug));
            }
            
            if (sObject == null)
            {
                sObject = new SObject(data.sdvId, 1);
            }
            Texture = bugModel.getTexture(Helper);
            id = bugModel.FullId;
            tileIndex = bugModel.TileIndex;
            bigCraftable.Value = false;
            type.Value = "Bug";
            //Monitor.Log(data.sdvId.ToString());
            ParentSheetIndex = data.sdvId;
            price.Value = bugModel.Price;
            bigCraftable.Value = false;
            tilesize = new Rectangle(0, 0, 16, 16);
            boundingBox.Value = new Rectangle(0, 0, tilesize.Width, tilesize.Height);
            name = bugModel.Name;
            Monitor.Log("bug built"+ name);
        }
      
        public override Item getOne()
        {
            Monitor.Log("getting one");
            return new Bug(data) { TileLocation = Vector2.Zero, name = name, Price = price, Quality = quality };
        }
       
        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, bool drawStackNumber, Color color, bool drawShadow)
        {
            //Game1.objectSpriteSheet.Tag = bugModel.FullId;
            spriteBatch.Draw(Texture, location + new Vector2((Game1.tileSize / 4 ), (Game1.tileSize / 4)), sourceRectangle, Color.White * transparency, 0.0f, new Vector2(8f, 8f), Game1.pixelZoom * (scaleSize < 0.2 ? scaleSize : scaleSize / 2.00f), SpriteEffects.None, layerDepth);

            if (drawStackNumber && maximumStackSize() > 1 && (scaleSize > 0.3 && Stack != int.MaxValue) && Stack > 1)
                Utility.drawTinyDigits(stack, spriteBatch, location + new Vector2((Game1.tileSize - Utility.getWidthOfTinyDigitString(stack, 3f * scaleSize)) + 3f * scaleSize, (float)(Game1.tileSize - 18.0 * scaleSize + 2.0)), 3f * scaleSize, 1f, Color.White);
        }
        public override void drawWhenHeld(SpriteBatch spriteBatch, Vector2 objectPosition, Farmer f)
        {
            //todo: could move this down a little more
            spriteBatch.Draw(Texture, objectPosition + new Vector2((Game1.tileSize / 4), (Game1.tileSize / 4)) , sourceRectangle, Color.White, 0.0f, Vector2.Zero, Game1.pixelZoom * 0.25f, SpriteEffects.None, Math.Max(0.0f, (f.getStandingY() + 2) / 10000f));
        }

        //public virtual void actionWhenBeingHeld(Farmer who)
        //{
        //}

        //public virtual void actionWhenStopBeingHeld(Farmer who)
        //{
        //}


        public override ICustomObject recreate(Dictionary<string, string> additionalSaveData, object replacement)
         {
              return (ICustomObject) CustomObjectData.collection[additionalSaveData["id"]].getObject();
         }

        public override bool canBeTrashed()
         {
                return true;
         }
    }
   
}
