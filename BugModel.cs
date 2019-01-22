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
        public int OriginalWidth { get; set; } = 16;
        public int Price { get; set; } = 100;
        public static Texture2D texture { get; set; }

        public Texture2D getTexture(IModHelper helper = null)
        {
             if (helper == null)
                helper = Helper;

            texture = Helper.Content.Load<Texture2D>(TextureAsset, ContentSource.ModFolder);
            Monitor.Log("got Texture");
            return texture;

        }

    }
    
}
