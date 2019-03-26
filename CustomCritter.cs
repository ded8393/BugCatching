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
        //private LightSource light;
        public int identifier;
        public float xVelocity;
        public float yVelocity;
        public CustomCritter(CritterEntry data)
        {
            this.data = data;
            this.flip = Game1.random.NextDouble() < 0.5;
            var tex = BugCatchingMod.instance.Helper.Content.Load<Texture2D>(data.BugModel.SpriteData.TextureAsset);
            var texStr = BugCatchingMod.instance.Helper.Content.GetActualAssetKey(data.BugModel.SpriteData.TextureAsset);

            this.baseFrame = data.BugModel.SpriteData.TileIndex;
            this.sprite = new AnimatedSprite(texStr, baseFrame, data.BugModel.SpriteData.FrameWidth, data.BugModel.SpriteData.FrameHeight);

        }

        public CustomCritter( Vector2 position, CritterEntry data )
            :this(data)
        {
            this.position = this.startingPosition = position;
      
        }
        public override void draw(SpriteBatch b)
        {
            this.sprite.draw(b, Game1.GlobalToLocal(Game1.viewport, this.position + new Vector2(-64f, -64f)), 0.0f, 0, 0, Color.White, this.flip, data.BugModel.SpriteData.Scale, 0.0f, false);
        }
        public override void drawAboveFrontLayer(SpriteBatch b)
        {
            this.sprite.draw(b, Game1.GlobalToLocal(Game1.viewport, this.position + new Vector2(-64f, this.yJumpOffset - 128f + this.yOffset)), this.position.Y / 10000f, 0, 0, Color.White, this.flip, data.BugModel.SpriteData.Scale, 0.0f, false);
        }
        public void setTrajectory(int xVelocity, int yVelocity)
        {
            this.setTrajectory(new Vector2((float)xVelocity, (float)yVelocity));
        }

        public virtual void setTrajectory(Vector2 trajectory)
        {
            this.xVelocity = trajectory.X;
            this.yVelocity = trajectory.Y;
        }

        protected void applyVelocity(GameLocation currentLocation)
        {
            Rectangle boundingBox = this.getBoundingBox(0, 0);
            boundingBox.X += (int)this.xVelocity;
            boundingBox.Y -= (int)this.yVelocity;
            if (currentLocation == null || !currentLocation.isCollidingPosition(boundingBox, Game1.viewport, false, 0, false))
            {
                this.position.X += this.xVelocity;
                this.position.Y -= this.yVelocity;
            }
            this.xVelocity = (float)(int)((double)this.xVelocity - (double)this.xVelocity / 2.0);
            this.yVelocity = (float)(int)((double)this.yVelocity - (double)this.yVelocity / 2.0);
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
    public class Crawler : CustomCritter
    {
        private int crawlTimer;
        private int crawlSpeed = 50;

        public Crawler(Vector2 position, CritterEntry data)
            :base(position, data)
        {
            this.sprite.loop = false;
            this.baseFrame = this.sprite.currentFrame;
            this.crawlSpeed = Game1.random.Next(200, 350);
        }
        public void doneWithJump(Farmer who)
        {
            this.crawlTimer = 200 + Game1.random.Next(-5, 6);
        }
        public override bool update(GameTime time, GameLocation environment)
        {
            this.crawlTimer -= time.ElapsedGameTime.Milliseconds;
            float motion = (float)Game1.random.NextDouble() / 2f;

            if (this.crawlTimer <= 0 && this.sprite.CurrentAnimation == null)
            {

                ////todo: add check for terrain features//debris
                this.position.X += this.flip ? -1f * motion : motion;

                List<FarmerSprite.AnimationFrame> Movement = new List<FarmerSprite.AnimationFrame>();
                for (int i = 1; i < this.data.Behavior.NumFrames; i++)
                {
                    this.crawlSpeed = Game1.random.Next(200, 350);
                    Movement.Add(new FarmerSprite.AnimationFrame(this.baseFrame + i, this.crawlSpeed));
                }

                Movement.Add(new FarmerSprite.AnimationFrame(this.baseFrame, this.crawlSpeed * 2, false, false, new AnimatedSprite.endOfAnimationBehavior(this.doneWithJump), false));
                this.sprite.setCurrentAnimation(Movement);

                this.crawlTimer = 200;
            }

            this.position.X += this.flip ? -1f * motion : motion;

            return base.update(time, environment);
        }
        public override void draw(SpriteBatch b)
        {
            this.sprite.draw(b, Game1.GlobalToLocal(Game1.viewport, this.position + new Vector2(-64f, -64f)), 0.0f, 0, 0, Color.White, this.flip, data.BugModel.SpriteData.Scale, 0.0f, false);
        }
    }
    
    
}
