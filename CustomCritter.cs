using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewValley.BellsAndWhistles;

namespace BugCatching
{
    // MIGHT RENAME CRITTERCLASSIFIER
    public class CustomCritter : Critter
    {
        public CritterEntry data; 
        private LightSource light;

        public CustomCritter(CritterEntry data)
        {
            this.data = data;
        }

        public CustomCritter( Vector2 pos, CritterEntry data )
        {
            this.position = this.startingPosition = pos;
            this.data = data;
            this.flip = Game1.random.NextDouble() < 0.5;
            var tex = BugCatchingMod.instance.Helper.Content.Load<Texture2D>(data.BugModel.SpriteData.TextureAsset);
            var texStr = BugCatchingMod.instance.Helper.Content.GetActualAssetKey(data.BugModel.SpriteData.TextureAsset);

            this.baseFrame = data.BugModel.SpriteData.TileIndex;
            this.sprite = new AnimatedSprite(texStr, baseFrame, data.BugModel.SpriteData.FrameWidth, data.BugModel.SpriteData.FrameHeight);

            if ( data.Light != null )
            {
                var col = new Color(255 - data.Light.Color.R, 255 - data.Light.Color.G, 255 - data.Light.Color.B);
                if (data.Light.VanillaLightId != -1)
                    light = new LightSource(data.Light.VanillaLightId, position, data.Light.Radius, col);
                else
                    light = new LightSource(4, position, data.Light.Radius, col);
                Game1.currentLightSources.Add(light);
            }         
        }
        public override void draw(SpriteBatch b)
        {
            this.sprite.draw(b, Game1.GlobalToLocal(Game1.viewport, this.position + new Vector2(-64f, -64f)), 0.0f, 0, 0, Color.White, this.flip, data.BugModel.SpriteData.Scale, 0.0f, false);
        }
        public override void drawAboveFrontLayer(SpriteBatch b)
        {
            this.sprite.draw(b, Game1.GlobalToLocal(Game1.viewport, this.position + new Vector2(-64f, this.yJumpOffset - 128f + this.yOffset)), this.position.Y / 10000f, 0, 0, Color.White, this.flip, data.BugModel.SpriteData.Scale, 0.0f, false);
        }
        public Critter getCritter()
        {
            Log.info($"getting Critter: {data.BugModel.FullId}");
            if (data.Behavior.Classification == "Flying")
                return new Floater(this);
            else if (data.Behavior.Classification == "Crawler")
                return new Crawler(this);
            else
                return new Crawler(this);
        }
        //public override bool update(GameTime time, GameLocation environment)
        //{
        //      if (light != null)
        //        light.position.Value = this.position;
        //}
    }

    public class Floater : Butterfly
    {
        private bool summerButterfly;
        public CritterEntry data { get; set; } = new CritterEntry();

        public Floater(CustomCritter critter)
            : base(critter.startingPosition)
        {
            this.data = critter.data;
            this.sprite = critter.sprite;
            this.position = critter.startingPosition;
            this.startingPosition = this.position;
            this.baseFrame = this.sprite.currentFrame;
            if (critter.data.Behavior.NumFrames == 4)
                this.summerButterfly = true;
            this.sprite.loop = false;
        }
        public override void drawAboveFrontLayer(SpriteBatch b)
        {
            this.sprite.draw(b, Game1.GlobalToLocal(Game1.viewport, this.position + new Vector2(-64f, this.yJumpOffset - 128f + this.yOffset)), this.position.Y / 10000f, 0, 0, Color.White, this.flip, data.BugModel.SpriteData.Scale, 0.0f, false);
        }
    }
    public class Crawler : Critter
    {
        public CritterEntry data { get; set; } = new CritterEntry();

        private int crawlTimer;
        private int crawlSpeed = 50;

        public Crawler(CustomCritter critter)
        {
            this.data = critter.data;
            this.sprite = critter.sprite;
            this.sprite.loop = false;
            this.flip = critter.flip;
            this.baseFrame = this.sprite.currentFrame;
            this.position = critter.startingPosition;
            this.crawlSpeed = Game1.random.Next(200, 350);
            this.startingPosition = this.position;
        }
        public void doneWithJump(Farmer who)
        {
            this.crawlTimer = 200 + Game1.random.Next(-5, 6);
        }
        public override bool update(GameTime time, GameLocation environment)
        {
            this.crawlTimer -= time.ElapsedGameTime.Milliseconds;
            if (this.crawlTimer <= 0 && this.sprite.CurrentAnimation == null)
            {

                ////todo: add check for terrain features//debris

                this.position.X += this.flip ? -8f : 8f;
                if (base.update(time, environment))
                {
                    this.flip = !this.flip;
                    this.position.X += this.flip ? -8f : 8f;
                }
                    
                this.sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
                {
                    new FarmerSprite.AnimationFrame(this.baseFrame + 1, this.crawlSpeed),
                    new FarmerSprite.AnimationFrame(this.baseFrame + 2, this.crawlSpeed ),
                    new FarmerSprite.AnimationFrame(this.baseFrame, this.crawlSpeed, false, false, new AnimatedSprite.endOfAnimationBehavior(this.doneWithJump), false)

                });
                this.crawlTimer = 200;
            }

            return base.update(time, environment);
        }
        public override void draw(SpriteBatch b)
        {
            this.sprite.draw(b, Game1.GlobalToLocal(Game1.viewport, this.position + new Vector2(-64f, -64f)), 0.0f, 0, 0, Color.White, this.flip, data.BugModel.SpriteData.Scale, 0.0f, false);
        }
    }
    
    
}
