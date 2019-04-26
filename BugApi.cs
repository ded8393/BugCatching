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
        internal static List<string> AllKnownClassifications = new List<string>() { "Floater", "Crawler" };




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
            var bug = new Bug();
            
            BugModel data = createBugModelFromCritter(critter);
            bug = new Bug(data);
            return bug;
        }

        public static BugModel createBugModelFromCritter(Critter critter)
        {
            string bugName = critter.GetType().ToString().Split('.').Last();
            if (AllKnownClassifications.Contains(bugName))
            {
                BugModel bugModel = new BugModel();
                if (bugName == "Floater")
                {
                    Floater f = (Floater) critter;
                    bugModel = AllBugs.Find(b => b.FullId == f.data.BugModel.FullId);
                }
                else
                {
                    CustomCritter c = (CustomCritter)critter;
                    bugModel = AllBugs.Find(b => b.FullId == c.data.BugModel.FullId);
                }
                return bugModel;
            }

            int TileIndex = Helper.Reflection.GetField<int>(critter, "baseFrame").GetValue();
            return createPlainBugModel(bugName, TileIndex);
           
        }

        public static BugModel createPlainBugModel(string bugName, int tileIndex)
        {
            string plainBugDescription = "Just a plain old " + bugName;
            int plainBugPrice = 100;
            string plainBugQuickItemString = bugName + "/100/-50/Bug/Just a Plain " + bugName + "/true/true/0/" + bugName;
            string plainBugTextureAsset = "Assets/critters.png";
            string[] plainBugIdList = { "Plain", bugName, tileIndex.ToString()};
            string plainBugId = String.Join(".", plainBugIdList);
            SpriteData plainBugSpriteData= new SpriteData() { TileIndex = tileIndex, TextureAsset = plainBugTextureAsset, FrameHeight = 16, FrameWidth = 16 };
            BugModel plainBugModel = new BugModel() { Name = bugName, Id = plainBugId, Description = plainBugDescription, Price = plainBugPrice, QuickItemDataString = plainBugQuickItemString, SpriteData = plainBugSpriteData };

            if (BugCatchingMod.AllBugs.Find(b => b.FullId == plainBugModel.FullId) != null)
            {
                Log.debug("Found bug in AllBugs");
                return plainBugModel;
            }
                
            else
            {
                Log.debug("Adding bug to AllBugs");
                BugCatchingMod.AllBugs.AddOrReplace(plainBugModel);
                return plainBugModel;
            }

        }


    }
}
