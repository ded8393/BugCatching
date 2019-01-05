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
    class Bug : PySObject
    {
        internal IModHelper Helper = BugNetMod._helper;
        internal IMonitor Monitor = BugNetMod._monitor;
        internal List<BugModel> AllBugs = BugNetMod.AllBugs;

        public BugModel bugModel;
        public virtual Texture2D Texture { get; private set; }
        public virtual Rectangle sourceRectangle => Game1.getSourceRectForStandardTileSheet(Texture, bugModel.TileIndex, tilesize.Width, tilesize.Height);

        private Rectangle tilesize = new Rectangle(0, 0, 16, 16);
        private int tileIndex;
        private string id;

        public Bug()
        {
            syncObject = new PySync(this);
            syncObject.init();
        }


        public Bug(CustomObjectData data)
        {
            this.data = data;
            checkData();
            sObject = new SObject(data.sdvId, 1);
            build(AllBugs.Find(b => b.FullId == data.id)); 
           
        }
        public Bug(CustomObjectData data, Vector2 tileLocation)
        {
            sObject = new SObject(tileLocation, data.sdvId);
            this.data = data;
            checkData();
            build(AllBugs.Find(b => b.FullId == data.id));
        }
        public Bug(Critter critter)
        {
            var critterType = critter.GetType().ToString().Split('.').Last();
            bugModel = AllBugs.Find(b => b.Name == critterType);
            //object privateFieldValue = critter.GetType().GetField("baseframe", BindingFlags.NonPublic | BindingFlags.Instance)
            //                .GetValue(location);
            bugModel.TileIndex = this.Helper.Reflection.GetField<int>(critter, "baseFrame").GetValue();
            Monitor.Log(bugModel.TileIndex.ToString());
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
        public override Color getCategoryColor()
        {
            return Color.LimeGreen;
        }

        private SObject maxed(SObject obj)
        {
            SObject o = (SObject)obj.getOne();
            o.Stack = int.MaxValue;
            return o;
        }
        public virtual string getCatName(int cat)
        {

            return "Bug";
        }
        public override string DisplayName { get => name; set => base.DisplayName = value; }

        public override string getDescription()
        {
            return Game1.parseText(bugModel.Description, Game1.smallFont, Game1.tileSize * 4 + Game1.tileSize / 4);
        }
        public virtual void build(BugModel bugModel)
        {
            Monitor.Log("building bug" + tileIndex);
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
            
            
            Texture = bugModel.getTexture(Helper);
            id = bugModel.FullId;
            tileIndex = bugModel.TileIndex;
            type.Value = "Bug";
            parentSheetIndex.Value = data.sdvId;
            bigCraftable.Value = false;
            tilesize = new Rectangle(0, 0, bugModel.OriginalWidth, bugModel.OriginalWidth);
            boundingBox.Value = new Rectangle(0, 0, tilesize.Width, tilesize.Height);
            name = bugModel.Name;
            Monitor.Log("bug built"+ tileIndex);
        }
      
        public override Item getOne()
        {
            Monitor.Log("getting one");
            return new Bug(data) { TileLocation = Vector2.Zero, name = name, Price = price, Quality = quality };
        }
        //public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, bool drawStackNumber, Color color, bool drawShadow)
        //{
        //    Monitor.Log("using drawInMenu " + tileIndex);
        //    //spriteBatch.Draw(Texture, location + new Vector2(32f, 32f), new Rectangle?(Game1.getSquareSourceRectForNonStandardTileSheet(Texture, 16, 16, tileIndex)), color * transparency, 0.0f, new Vector2(8f, 8f), 4f * scaleSize, SpriteEffects.None, layerDepth);
        //    spriteBatch.Draw(Texture, location + new Vector2((Game1.tileSize / 2), (Game1.tileSize / 2)), sourceRectangle, Color.White * transparency, 0.0f, new Vector2(8f,8f) * scaleSize, Game1.pixelZoom * (scaleSize < 0.2 ? scaleSize : scaleSize / 2.00f), SpriteEffects.None, layerDepth);

        //}
        public override void drawWhenHeld(SpriteBatch spriteBatch, Vector2 objectPosition, Farmer f)
        {
            Monitor.Log("drawWhenHeld");
            spriteBatch.Draw(Texture, objectPosition, sourceRectangle, Color.White, 0.0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, Math.Max(0.0f, (f.getStandingY() + 2) / 10000f));
        }
        public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1)
        {
            Monitor.Log("using draw 4 arg");
            Vector2 vector2 = getScale() * Game1.pixelZoom;
            Vector2 local = Game1.GlobalToLocal(Game1.viewport, new Vector2((x * Game1.tileSize), (y * Game1.tileSize - Game1.tileSize)));
            var r = data.sourceRectangle;
            Rectangle destinationRectangle = new Rectangle((int)(local.X - vector2.X / 2.0) + (shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0), (int)(local.Y - vector2.Y / 2.0) + (shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0), (int)((bugModel.OriginalWidth * 4) + (double)vector2.X), (int)((bugModel.OriginalWidth * 4) + vector2.Y / 2.0));
            if (Texture is ScaledTexture2D s)
            {
                Rectangle tilesize = data.bigCraftable ? new Rectangle(0, 0, 16, 32) : new Rectangle(0, 0, 16, 16);
                var newSR = new Rectangle?(new Rectangle((int)(r.X * s.Scale), (int)(r.Y * s.Scale), (int)(r.Width * s.Scale), (int)(r.Height * s.Scale)));
                spriteBatch.Draw(s.STexture, destinationRectangle, newSR, Color.White * alpha, 0.0f, Vector2.Zero, SpriteEffects.None, (float)(Math.Max(0.0f, ((y + 1) * Game1.tileSize - Game1.pixelZoom * 6) / 10000f) + x * 9.99999974737875E-06));
            }
            else
                //spriteBatch.Draw(Texture,destinationRectangle, r, Color.White * alpha, 0.0f,Vector2.Zero, SpriteEffects.None, (float)(Math.Max(0.0f, ((y + 1) * Game1.tileSize - Game1.pixelZoom * 6) / 10000f) + x * 9.99999974737875E-06));
                base.draw(spriteBatch, x, y, alpha);
        }
        public override void draw(SpriteBatch spriteBatch, int xNonTile, int yNonTile, float layerDepth, float alpha = 1)
        {
            if (data.texture is ScaledTexture2D)
                draw(spriteBatch, xNonTile, yNonTile, alpha);
            else
                base.draw(spriteBatch, xNonTile, yNonTile, layerDepth, alpha);
        }
        //public override void draw(SpriteBatch spriteBatch, int xNonTile, int yNonTile, float layerDepth, float alpha = 1)
        //{
        //    Monitor.Log("using draw 5 args");
        //    draw(spriteBatch, xNonTile, yNonTile, alpha);
        //}
        //public virtual void actionWhenBeingHeld(Farmer who)
        //{
        //}

        //public virtual void actionWhenStopBeingHeld(Farmer who)
        //{
        //}
        public override ICustomObject recreate(Dictionary<string, string> additionalSaveData, object replacement)
        {
            return (ICustomObject)CustomObjectData.collection[additionalSaveData["id"]].getObject();
        }
        
    }
   
}
