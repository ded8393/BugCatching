using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.BellsAndWhistles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugCatching
{
    public class CustomCritter : Critter
    {
        public CritterEntry data;
        private LightSource light;
        private Random rand;

        public CustomCritter(CritterEntry data)
        {
            this.data = data;
        }

        public CustomCritter( Vector2 pos, CritterEntry data )
        {
            this.position = this.startingPosition = pos;
            this.data = data;
            this.rand = new Random(((int)startingPosition.X) << 32 | ((int)startingPosition.Y));

            var tex = BugCatchingMod.instance.Helper.Content.Load<Texture2D>(data.BugModel.TextureAsset);
            var texStr = BugCatchingMod.instance.Helper.Content.GetActualAssetKey(data.BugModel.TextureAsset);

            this.baseFrame = data.BugModel.TileIndex;
            List<FarmerSprite.AnimationFrame> frames = new List<FarmerSprite.AnimationFrame>();
            foreach (var frame in data.Animations["default"].Frames)
            {
                frames.Add(new FarmerSprite.AnimationFrame(baseFrame + frame.Frame, frame.Duration));
            }
            this.sprite = new AnimatedSprite(texStr, baseFrame, data.SpriteData.FrameWidth, data.SpriteData.FrameHeight);
            sprite.setCurrentAnimation(frames);
            //data.Behavior.sprite = new AnimatedSprite(texStr, baseFrame, data.SpriteData.FrameWidth, data.SpriteData.FrameHeight);
            //data.Behavior.startingPosition = this.startingPosition;


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
        

        public override bool update(GameTime time, GameLocation environment)
        {
           

            if (light != null)
                light.position.Value = this.position;
            //if (data.Behavior.isClassified)
            //{
            //    return (bool)data.Behavior.update(time, environment);
            //}
            //else
                return base.update(time, environment);
        }

        public override void draw(SpriteBatch b)
        {

            if (data == null)
                return;

            //base.draw(b);
            float z = (float)((double)this.position.Y / 10000.0 + (double)this.position.X / 100000.0);
            if (!data.SpriteData.Flying)
                z = (float)((this.position.Y - 1.0) / 10000.0);
            this.sprite.draw(b, Game1.GlobalToLocal(Game1.viewport, this.position - new Vector2(8, 8)), z, 0, 0, Color.White, this.flip, data.SpriteData.Scale, 0.0f, false);
        }
        
    }
}
