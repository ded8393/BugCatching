using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewModdingAPI;    

namespace BugCatching.Classifications
{
    public class Crawler : CustomCritter
    {
        private int crawlTimer;
        private int crawlSpeed = 50;

        public Crawler(GameLocation location, Vector2 position, CritterEntry data)
            : base(location, position, data)
        {
            this.sprite.loop = false;
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
            this.sprite.draw(b, Game1.GlobalToLocal(Game1.viewport, this.position + new Vector2(-64f, -64f)), 1.0f, 0, 0, Color.White, this.flip, data.BugModel.SpriteData.Scale, 0.0f, false);
        }
    }
    //public class Floater : Butterfly
    //{
    //    private bool summerButterfly;
    //    public CritterEntry data { get; set; } = new CritterEntry();

    //    public Floater(CustomCritter critter)
    //        : base(critter.startingPosition)
    //    {
    //        this.data = critter.data;
    //        this.sprite = critter.sprite;
    //        this.position = critter.startingPosition;
    //        this.startingPosition = this.position;
    //        this.baseFrame = this.sprite.currentFrame;
    //        if (critter.data.Behavior.NumFrames == 4)
    //            this.summerButterfly = true;
    //        this.sprite.loop = false;
    //    }
    //    public override void drawAboveFrontLayer(SpriteBatch b)
    //    {
    //        this.sprite.draw(b, Game1.GlobalToLocal(Game1.viewport, this.position + new Vector2(-64f, this.yJumpOffset - 128f + this.yOffset)), this.position.Y / 10000f, 0, 0, Color.White, this.flip, data.BugModel.SpriteData.Scale, 0.0f, false);
    //    }
    //}
}
