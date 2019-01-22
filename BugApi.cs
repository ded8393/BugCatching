using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI;
using PyTK.Extensions;
using PyTK.CustomElementHandler;

using StardewValley;
using Critter = StardewValley.BellsAndWhistles.Critter;

namespace BugNet
{
    public class BugApi
    {
        private static IModHelper Helper;
        internal List<BugModel> AllBugs = BugNetMod.AllBugs;
        internal static int Count = 0;

        public static void init(IModHelper helper)
        {
            Helper = helper;
        }

        public Bug getBugFromCritterType(Critter critter)
        {
            var bug = new Bug();
            
            BugModel data = getDataFromCritter(critter);
            bug = new Bug(data);
            Count++;
            return bug;
        }
        public BugModel getDataFromCritter(Critter critter)
        {
            string bugName = critter.GetType().ToString().Split('.').Last();
            string bugId = "Plain." + Count.ToString();
            string Description = "Just a plain old " + bugName;
            int Price = 100;
            string QuickItemString = bugName + "/100/-50/Bug/Just a Plain " + bugName + "/true/true/0/" + bugName;
            string TextureAsset = "Assets/critters.png";

            //AnimatedSprite Sprite = Helper.Reflection.GetField<AnimatedSprite>(critter, "sprite").GetValue();
            //Texture2D Texture = Sprite.Texture;

            int TileIndex = Helper.Reflection.GetField<int>(critter, "baseFrame").GetValue();

            BugModel bugModel = new BugModel() { Name = bugName, Id = bugId, Description = Description, TileIndex = TileIndex, Price = Price, TextureAsset = TextureAsset, QuickItemDataString = QuickItemString };
            CustomObjectData.newObject(bugModel.FullId, bugModel.getTexture(), Color.White, bugModel.Name, bugModel.Description, bugModel.TileIndex, price: bugModel.Price, customType: typeof(Bug));
            BugNetMod.AllBugs.AddOrReplace(bugModel);
            return bugModel;
        }
    }
}
