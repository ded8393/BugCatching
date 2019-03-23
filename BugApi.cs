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

using BugCatching.Classifications;


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
            catch
            {
                Log.info("Parsing Name to Create Bug: " + bugId);
                List<string> i = bugId.Split('.').ToList();
                string tileIndex = i.Last();
                string bugName = i[(i.IndexOf(tileIndex) - 1)];
                Log.info("Parsed name: " + bugName + " tileIndex: " + tileIndex);
                bugModel = createPlainBugModel(bugName, tileIndex.toInt());
                return bugModel;
            }     
        }

        public static Bug getBugFromCritterType(Critter critter)
        {
            BugModel data = createBugModelFromCritter(critter);
            return new Bug(data);
        }

        public static BugModel createBugModelFromCritter(Critter critter)
        {
            string bugName = critter.GetType().ToString().Split('.').Last();
            if (Behavior.AllClassifications.Contains(bugName))
            {
                 CustomCritter f = (CustomCritter)critter;
                 return (BugModel) f.data.BugModel;
           }
            int TileIndex = Helper.Reflection.GetField<int>(critter, "baseFrame").GetValue();

            return createPlainBugModel(bugName, TileIndex);
           
        }

        public static BugModel createPlainBugModel(string bugName, int tileIndex)
        {
            string plainBugDescription = $"Just a plain old {bugName}";
            string plainBugQuickItemString = $"{bugName}/100/-50/Bug/Just a Plain {bugName}/true/true/0/{bugName}";

            SpriteData plainBugSpriteData= new SpriteData() { TileIndex = tileIndex, TextureAsset = "Assets/critters.png", FrameHeight = 16, FrameWidth = 16 };
            BugModel plainBugModel = new BugModel() { Name = bugName, Id = $"Plain.{bugName}.{tileIndex}", Description = plainBugDescription, Price = 100, QuickItemDataString = plainBugQuickItemString, SpriteData = plainBugSpriteData };

            if (BugCatchingMod.AllBugs.Find(b => b.FullId == plainBugModel.FullId) != null)
                return plainBugModel;
            else
            {
                Log.debug("Adding bug to AllBugs");
                BugCatchingMod.AllBugs.AddOrReplace(plainBugModel);
                return plainBugModel;
            }

        }

        public static CustomCritter getCustomCritter(GameLocation location, CritterEntry data, Vector2 position)
        {
            switch (data.Behavior.Classification)
            {
                case "Crawler":
                    return new Crawler(location, position, data);
                case "Floater":

                default:
                    return new CustomCritter(location, position, data);
            }
        }


    }
    //public static class CritterFactory
    //{
    //    public static Crawler GetCrawler(Vector2 position, CritterEntry data)
    //    {
    //        return new Crawler(position, data);
    //    }

    //}
}
