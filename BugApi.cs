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

namespace BugCatching
{
    public static class BugApi
    {
        private static IModHelper Helper;
        internal static List<BugModel> AllBugs = BugCatchingMod.AllBugs;
        internal static int Count = 0;

        public static void init(IModHelper helper)
        {
            Helper = helper;
        }

        public static BugModel findOrCreateBugModelFromId(string bugId)
        {
            BugModel bugModel = new BugModel();

            try
            {
                bugModel = AllBugs.Find(bm => bugId == bm.FullId);
                Log.info("bugname _indb" + bugModel.Name.ToString());

                return bugModel;
            }
            catch { }
            

            if (bugId.Contains("Plain"))
            {
                List<string> i = bugId.Split('.').ToList();
                string tileIndex = i.Last();
                string bugName = i[(i.IndexOf(tileIndex) - 1)];
                bugModel = createPlainBugModel(bugName, tileIndex.toInt());
            }
            Log.info("bugname _plain" + bugModel.Name.ToString());

            return bugModel;
                
        }

        public static Bug getBugFromCritterType(Critter critter)
        {
            var bug = new Bug();
            
            BugModel data = getDataFromCritter(critter);
            bug = new Bug(data);
            return bug;
        }

        public static BugModel getDataFromCritter(Critter critter)
        {
            string bugName = critter.GetType().ToString().Split('.').Last();
            if (bugName == "Floater")
            {
                 Floater f = (Floater)critter;
                 return (BugModel) f.data.BugModel;
           }
            int TileIndex = Helper.Reflection.GetField<int>(critter, "baseFrame").GetValue();

            return createPlainBugModel(bugName, TileIndex);
           
        }

        public static BugModel createPlainBugModel(string bugName, int tileIndex)
        {
            string Description = "Just a plain old " + bugName;
            int Price = 100;
            string QuickItemString = bugName + "/100/-50/Bug/Just a Plain " + bugName + "/true/true/0/" + bugName;
            string TextureAsset = "Assets/critters.png";
            string[] bugIdList = { "Plain", bugName, tileIndex.ToString()};
            string bugId = String.Join(".", bugIdList);
            BugModel bugModel = new BugModel() { Name = bugName, Id = bugId, Description = Description, TileIndex = tileIndex, Price = Price, TextureAsset = TextureAsset, QuickItemDataString = QuickItemString };
            if (BugCatchingMod.AllBugs.Find(b => b.FullId == bugModel.FullId) != null)
                return bugModel;
            else
            {
                //CustomObjectData.newObject(bugModel.FullId, bugModel.getTexture(), Color.White, bugModel.Name, bugModel.Description, bugModel.TileIndex, price: bugModel.Price, customType: typeof(Bug));
                BugCatchingMod.AllBugs.AddOrReplace(bugModel);
                return bugModel;
            }

        }


    }
}
