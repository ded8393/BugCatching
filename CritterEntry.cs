using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.BellsAndWhistles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
namespace BugNet
{

    public class CritterEntry
    {
        public static IMonitor Monitor = BugNetMod._monitor;


        public BugModel BugModel { get; set; } = new BugModel();
        public class SpriteData_
        {
            public int Variations { get; set; }
            public int FrameWidth { get; set; }
            public int FrameHeight { get; set; }
            public float Scale { get; set; } = 4;
            public Boolean Flying { get; set; } = true;
        }
        public SpriteData_ SpriteData { get; set; } = new SpriteData_();

        public class Animation_
        {
            public class AnimationFrame_
            {
                public int Frame;
                public int Duration;
            }

            public List<AnimationFrame_> Frames = new List<AnimationFrame_>();
        }
        public Dictionary<string, Animation_> Animations { get; set; } = new Dictionary<string, Animation_>();

        public SpawnConditions SpawnConditions { get; set; } = new SpawnConditions();

        public class Behavior_
        {
            public string Type { get; set; }
            public float Speed { get; set; }
            
            public class PatrolPoint_
            {
                public string Type { get; set; } = "start";
                public float X { get; set; }
                public float Y { get; set; }
            }
            public List<PatrolPoint_> PatrolPoints { get; set; } = new List<PatrolPoint_>();
            public int PatrolPointDelay { get; set; }
            public int PatrolPointDelayAddRandom { get; set; }
        }
        public Behavior_ Behavior { get; set; }



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

        public virtual Vector2? attemptSpawn( GameLocation loc )
        {
            int attempts = SpawnAttempts;
            while (attempts > 0)
            {
                var spawnSpot = SpawnConditions.checkLocation(loc);
                if (spawnSpot != null)
                    return spawnSpot;
                attempts--;
            }

            return null;
        }

        public virtual Critter makeCritter()
        {
            return new CustomCritter(this);
        }

        public virtual Critter makeCritter(Vector2 pos)
        {
            return new CustomCritter(pos + new Vector2( 1, 1 ) *  (Game1.tileSize / 2), this);
        }

        internal static Dictionary<string, CritterEntry> critters = new Dictionary<string, CritterEntry>();
        public static void Register( CritterEntry entry )
        {
            critters.Add(entry.BugModel.Id, entry);
        }
    }
}
