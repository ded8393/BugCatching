using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.BellsAndWhistles;


namespace BugCatching
{
    public class Behavior
    {
        public string Classification { get; set; } = "Flying";
        public int NumFrames { get; set; } = 3;
        public static List<string> AllClassifications = new List<string> { "Floater", "Crawler", "Jumper" };

        public List<FarmerSprite.AnimationFrame> getMovement(int baseFrame, int movementSpeed)
        {
            List<FarmerSprite.AnimationFrame> Movement = new List<FarmerSprite.AnimationFrame>();
            for (int i = 1; i < this.NumFrames; i++)
                Movement.Add(new FarmerSprite.AnimationFrame(baseFrame + i, movementSpeed));
            if (Classification == "Floater")
            {
                for (int i = NumFrames - 1; i >= 1; i--)
                    Movement.Add(new FarmerSprite.AnimationFrame(baseFrame + i, movementSpeed));
            }
               
            Log.debug("got Movement");
            return Movement;
        }

    }
}
