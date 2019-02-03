using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.BellsAndWhistles;

namespace BugCatching
{
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

            var tex = BugCatchingMod.instance.Helper.Content.Load<Texture2D>(data.BugModel.TextureAsset);
            var texStr = BugCatchingMod.instance.Helper.Content.GetActualAssetKey(data.BugModel.TextureAsset);

            this.baseFrame = data.BugModel.TileIndex;
            this.sprite = new AnimatedSprite(texStr, baseFrame, data.SpriteData.FrameWidth, data.SpriteData.FrameHeight);

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
            this.sprite.draw(b, Game1.GlobalToLocal(Game1.viewport, this.position + new Vector2(-64f, this.yJumpOffset - 128f + this.yOffset)), this.position.Y / 10000f, 0, 0, Color.White, this.flip, 1f, 0.0f, false);
        }
        

    }
}
