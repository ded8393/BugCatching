using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI;


namespace BugCatching
{
    public class BugModel
    {
        public string Id { get; set; }
        public string FullId { get { return $"{BugCatchingMod.ModId}.{Id}"; } }
        public string Name { get; set; }
        public string Description { get; set; }
        public string QuickItemDataString { get; set; }

        public double Rarity { get; set; } = 1.0;
        public int Price { get; set; } = 100;
        //todo: test if ParentSheetIndex is needed
        public int ParentSheetIndex { get; set; }
        public SpriteData SpriteData { get; set; } = new SpriteData();

    }

    public class SpriteData
    {
        public string TextureAsset { get; set; }
        public int TileIndex { get; set; } = 0;
        public int Variations { get; set; } = 0;
        public int FrameWidth { get; set; } = 32;
        public int FrameHeight { get; set; } = 32;
        public float Scale { get; set; } = 1;
        public static Texture2D texture { get; set; }

	//todo: remove param; it's not used
        public Texture2D getTexture(IModHelper helper = null)
        {
            texture = BugCatchingMod._helper.Content.Load<Texture2D>(TextureAsset, ContentSource.ModFolder);
            if (texture != null)
                Log.info("got Texture");
            return texture;
        }
    }

}
