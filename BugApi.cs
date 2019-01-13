using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Critter = StardewValley.BellsAndWhistles.Critter;

namespace BugNet
{
    public class BugApi
    {
        internal List<BugModel> AllBugs = BugNetMod.AllBugs;

        public Bug getBugFromCritterType(Critter critter)
        {
            var bugName = critter.GetType().ToString().Split('.').Last();
            var bug = new Bug(AllBugs.Find(b => b.Name == bugName));
            return bug;
        }
    }
}
