using System;
using System.Windows;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewValley.BellsAndWhistles;
using BugCatching.Classifications;

using PyTK.Extensions;

namespace BugCatching
{
    // MIGHT RENAME CRITTERCLASSIFIER
    public class CustomCritter : Critter
    {
        public CritterLocation Home;
        public CritterEntry data;
        private LightSource light;
        private int actionTimer;
        private Vector2 baseStep = new Vector2();
        private Vector2 nextStep = new Vector2();

        private float maximumDeviation = 2f;
        private int movementSpeed = 50;

        private float motionMultiplier = 1f;

        private float Rotation;
        private List<FarmerSprite.AnimationFrame> Movement = new List<FarmerSprite.AnimationFrame>();

        public CustomCritter(CritterEntry data)
        {
            this.data = data;
        }

        public CustomCritter(GameLocation location, Vector2 pos, CritterEntry data)
            :this(data)
        {
            this.position = this.startingPosition = pos;
            this.Home = new CritterLocation(location, data) { layerName = "Front" };
            this.Home.getNextLocation();
            this.flip = Game1.random.NextDouble() < 0.5;

            var tex = BugCatchingMod.instance.Helper.Content.Load<Texture2D>(data.BugModel.SpriteData.TextureAsset);
            var texStr = BugCatchingMod.instance.Helper.Content.GetActualAssetKey(data.BugModel.SpriteData.TextureAsset);

            this.movementSpeed = Game1.random.Next(45, 80);

            this.Movement = this.data.Behavior.getMovement(this.baseFrame, this.movementSpeed);
            this.Movement.Add(new FarmerSprite.AnimationFrame(this.baseFrame, this.movementSpeed, false, false, new AnimatedSprite.endOfAnimationBehavior(this.doneWithAction), false));
            this.baseFrame = data.BugModel.SpriteData.TileIndex;
            this.sprite = new AnimatedSprite(texStr, baseFrame, data.BugModel.SpriteData.FrameWidth, data.BugModel.SpriteData.FrameHeight);
            this.baseStep = this.Home.getStepVectorToDestination(this.position, this.movementSpeed, this.maximumDeviation);
            this.sprite.loop = false;
            

            if (data.Light != null)
            {
                var col = new Color(255 - data.Light.Color.R, 255 - data.Light.Color.G, 255 - data.Light.Color.B);
                if (data.Light.VanillaLightId != -1)
                    light = new LightSource(data.Light.VanillaLightId, position, data.Light.Radius, col);
                else
                    light = new LightSource(4, position, data.Light.Radius, col);
                Game1.currentLightSources.Add(light);
            }
        }

        public void doneWithAction(Farmer who)
        {
            this.actionTimer = 200 + Game1.random.Next(-5, 6);
        }

        public override void draw(SpriteBatch b)
        {
            //this.sprite.draw(b, Game1.GlobalToLocal(Game1.viewport, this.position + new Vector2(-64f, -64f)), 0.0f, 0, 0, Color.White, this.flip, data.BugModel.SpriteData.Scale, 0.0f, false);
        }
        public override void drawAboveFrontLayer(SpriteBatch b)
        {
            if (this.sprite == null)
            {
                Log.debug("sprite is null bitch");
            }
            this.sprite.draw(b, Game1.GlobalToLocal(Game1.viewport, this.position + new Vector2(-64f, -64f)), 1.0f, 0, 0, Color.White, this.flip, data.BugModel.SpriteData.Scale, 0f, false);
        }

        public override bool update(GameTime time, GameLocation environment)
        {
            this.actionTimer -= time.ElapsedGameTime.Milliseconds;
            if (this.Home.tilePosition == null || this.Home.tilePosition == Vector2.Zero || this.Home.tilePosition == this.position)
            {
                Log.info("need new position");
                this.Home.getNextLocation();

            }
            

            if (this.actionTimer <= 0 && this.sprite.CurrentAnimation == null)
            {
                this.nextStep = this.baseStep;
                this.nextStep.X = this.baseStep.X + (float)(Game1.random.NextDouble() * Game1.random.Next(-1*(int)this.maximumDeviation, (int)this.maximumDeviation));

                if (this.data.Behavior.Classification == "Floater")
                    this.position.Y += (float)(Game1.random.NextDouble() * Game1.random.Next(-1 * (int)this.maximumDeviation, (int)this.maximumDeviation));
                var nextPosition = this.position + nextStep;
                if (environment.isTileOccupiedIgnoreFloors(nextPosition))
                    if (!environment.isTileOccupiedIgnoreFloors(new Vector2(this.position.X, nextPosition.Y)))
                        this.nextStep.X = 0;
                    else if (!environment.isTileOccupiedIgnoreFloors(new Vector2(nextPosition.X, this.position.Y)))
                        this.nextStep.Y = this.baseStep.Y * (float)(Game1.random.NextDouble() * Game1.random.Next(-1 * (int)this.maximumDeviation, (int)this.maximumDeviation));

                this.sprite.setCurrentAnimation(this.Movement);
                this.actionTimer = 200;
            }
            this.position += this.nextStep * this.motionMultiplier;
            this.baseStep.Y += 0.005f * (float)time.ElapsedGameTime.Milliseconds;
            this.motionMultiplier -= 0.0005f * (float)time.ElapsedGameTime.Milliseconds;
            if ((double)this.motionMultiplier <= 0.0)
                this.motionMultiplier = 0.0f; 
            //this.Rotation = (float) Math.Atan2((double)this.destination.Y - this.position.Y, (double)this.destination.X - this.position.X);
            //if (this.Rotation < 0)
            //    this.flip = !this.flip;
            return base.update(time, environment);
            //if (light != null)
            //    light.position.Value = this.position;
        }

    }

    
   
    
    
}
