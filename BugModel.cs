using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using Critter = StardewValley.BellsAndWhistles.Critter;
using PyTK.Types;
using PyTK.Extensions;

namespace BugNet
{
    public class BugModel
    {
        internal IModHelper Helper = BugNetMod._helper;
        internal IMonitor Monitor = BugNetMod._monitor;

        public string Name { get; set; }
        public string Id { get; set; }
        public string FullId
        {
            get {
                return $"ded.BugNet.{Id}";
            }
        } 
        public string Description { get; set;  }
        public string QuickItemDataString { get; set; }
        public string TextureAsset { get; set; }
        public int TileIndex { get; set; } = 0;
        public bool ScaleUp { get; set; } = false;
        public int OriginalWidth { get; set; } = 16;
        public int Price { get; set; } = 100;
        public string Season { get; set; } = null;
        public string TimeOfDay { get; set; } = null;
        public static Texture2D texture { get; set; }

        public Texture2D getTexture(IModHelper helper = null)
        {
            //if (texture != null)
            //    return texture;
            if (helper == null)
                helper = Helper;
            
                
            texture = Helper.Content.Load<Texture2D>(TextureAsset, ContentSource.ModFolder);
                
            if (ScaleUp)
            {
                float scale = (float)(Convert.ToDouble(texture.Width) / Convert.ToDouble(OriginalWidth));
                int height = (int)(texture.Height / scale);
                texture = ScaledTexture2D.FromTexture(texture.getArea(new Rectangle(0, 0, OriginalWidth, height)), texture, scale);
            }
            Monitor.Log("got Texture");
            return texture;

        }

    }
    
}
