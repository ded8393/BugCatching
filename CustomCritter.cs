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
        public Critter getCritter()
        {
            //if (Classification == "Flying")
            return new Floater(this);

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
    public class Jumper : Critter
    {
        public CritterEntry data { get; set; } = new CritterEntry();
        public Jumper(CustomCritter critter)
        {
            this.data = critter.data;
            this.sprite = critter.sprite;
            this.flip = critter.flip;
            this.baseFrame = this.sprite.currentFrame;
            this.position = critter.startingPosition;
            this.startingPosition = this.position;
            this.sprite.loop = true;
            this.sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
            {
                new FarmerSprite.AnimationFrame(300, 600),
                new FarmerSprite.AnimationFrame(304, 100),
                new FarmerSprite.AnimationFrame(305, 100),
                new FarmerSprite.AnimationFrame(306, 300),
                new FarmerSprite.AnimationFrame(305, 100),
                new FarmerSprite.AnimationFrame(304, 100)
            });
        }
    }
    
}
