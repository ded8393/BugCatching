using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.BellsAndWhistles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using StardewModdingAPI;
namespace BugCatching
{

    public class CritterEntry
    {
        public static IMonitor Monitor = BugCatchingMod._monitor;

        public BugModel BugModel { get; set; } = new BugModel();
       

        public SpawnConditions SpawnConditions { get; set; } = new SpawnConditions();

        public Behavior Behavior { get; set; } = new Behavior();
        public int SpawnAttempts { get; set; } = 3;

        public class Light_
        {
            public int VanillaLightId = 3;
            public float Radius { get; set; } = 0.5f;
            public class Color_
            {
                public int R { get; set; } = 255;
                public int G { get; set; } = 255;
                public int B { get; set; } = 255;
            }
            public Color_ Color { get; set; } = new Color_();
        }
        public Light_ Light { get; set; } = null;

        public virtual List<Vector2> attemptSpawn( GameLocation loc )
        {
            List<Vector2> spawnSpots = new List<Vector2>();
            for(int attempts = SpawnAttempts ; attempts > 0 ; attempts--)
            {
                var spawnSpot = SpawnConditions.checkLocation(loc);
                if (spawnSpot != null)
                    spawnSpots.Add((Vector2)spawnSpot);
            }
            return spawnSpots;
        }

        public virtual Critter makeCritter(Vector2 tile)
        {
            // absolute position
            var position = tile * Game1.tileSize;
            CustomCritter critter = new CustomCritter(position, this);
            if (this.BugModel.Classification == "Flying")
                return new Floater(critter);
            else if (this.BugModel.Classification == "Crawler")
                return new Crawler(position, this);
            else
                return new Crawler(position, this);

        }

        internal static Dictionary<string, CritterEntry> critters = new Dictionary<string, CritterEntry>();

        public static void Register( CritterEntry entry )
        {
            critters.Add(entry.BugModel.Id, entry);
        }
    }
}
