using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Threading.Tasks;

using PyTK.Types;


using StardewModdingAPI;
namespace BugCatching
{

    public class CritterEntry
    {
        public static IMonitor Monitor = BugCatchingMod._monitor;

        public BugModel BugModel { get; set; } = new BugModel();
        public string Classification { get; set; } = "Crawler";

        public List<Attractor> Attractors { get; set; } = new List<Attractor>();

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

        public virtual Critter makeCritter(Vector2 tile)
        {
            // absolute position
            var position = tile * Game1.tileSize;
            CustomCritter critter = new CustomCritter(position, this);
            if (Classification == "Floater")
                return new Floater(critter);
            else if (Classification == "Crawler")
                return new Crawler(position, this);
            else
                return new Crawler(position, this);

        }

        internal static Dictionary<string, CritterEntry> critters = new Dictionary<string, CritterEntry>();

        public static void Register( CritterEntry entry )
        {
            critters.Add(entry.BugModel.Id, entry);
        }

        public static string GetCritterIdFromRegistry(CritterEntry entry)
        {
            return critters.Where(c => c.Value == entry).Single().Key;
        }

        public static explicit operator CritterEntry(CustomCritter critter)
        {
            return critter.data;
        }
    }
    public class Attractor
    {
        public string AttractorType { get; set; } = "random";
        public string AttractorName { get; set; } = "";
        public string PropertyName { get; set; } = "";
        public string IsType { get; set; } = "";
        public string IsValue { get; set; } = "";

        public List<Vector2> getAttractorTilesAtLocation(GameLocation Location)
        {

            List<Vector2> viableTiles = new List<Vector2>();
            if (AttractorType == "random")
            {
                foreach (int i in new Range(10).toList())
                    viableTiles.Add(Location.getRandomTile());
                return viableTiles;
            }
            else if (AttractorType == "terrainfeature")
            {
                foreach (var location in new TerrainSelector<TerrainFeature>(T => T.GetType().Name == AttractorName).keysIn(Location))
                {
                    if (validateAttractor(Location.terrainFeatures[location]))
                        viableTiles.Add(location);
                }
                Log.info($"there were ({viableTiles.Count}) of {Location.terrainFeatures[viableTiles.First()].GetType().Name} at {Location.Name}");
                return viableTiles;
            }
            else if (AttractorType == "object")
            {
                var keys = Location.objects.Keys.ToList();
                keys.Shuffle();
                foreach (var key in keys)
                {
                    string objectName = Location.objects[key].displayName;
                    if (AttractorName != null && AttractorName != "" && AttractorName == objectName)
                        if (validateAttractor(Location.objects[key]))
                            viableTiles.Add(key);
                }
                Log.info($"there were ({viableTiles.Count}) of {Location.objects[viableTiles.First()].displayName} at {Location.Name}");
                return viableTiles;
            }

            else throw new ArgumentException("Bad Attractor type");
        }

        public bool validateAttractor(object obj)
        {
            bool isMatch = true;

            if (PropertyName != null && PropertyName != "")
            {
                var o = obj;
                string[] toks = PropertyName.Split('.');
                for (int i = 0; i < toks.Length; ++i)
                {
                    if (o == null)
                        break;

                    var f = o.GetType().GetField(toks[i], BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                    if (f == null)
                    {
                        o = null;
                        break;
                    }
                    o = f.GetValue(o);
                }

                if (o != null)
                {
                    if (IsType != null && IsType != "" && !o.GetType().IsInstanceOfType(Type.GetType(IsType)))
                        isMatch = false;
                    else if (IsValue != null && IsValue != "" && !o.ToString().Equals(IsValue))
                        isMatch = false;
                }
            }
            return isMatch;
        }

    }
}
