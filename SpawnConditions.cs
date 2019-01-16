using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;


using StardewValley;

namespace BugNet
{
    public class SpawnConditions
    {
        public string[] Seasons { get; set; } = new string[0];
        public string[] Locations { get; set; } = new string[0];
        public int MinTimeOfDay { get; set; } = -1;
        public int MaxTimeOfDay { get; set; } = -1;
        public bool RequireDarkOut { get; set; } = false;
        public bool AllowRain { get; set; } = false;
        public List<Attractor> Attractors { get; set; } = new List<Attractor>();

        public class Attractor
        {
            public string AttractorType { get; set; } = "random";
            public string AttractorName { get; set; } = "";
            public string PropertyName { get; set; } = "";
            public string IsType { get; set; } = "";
            public string IsValue { get; set; } = "";
            
            //derived from pickSpot
            public Vector2? checkLocation(GameLocation loc)
            {
                if (AttractorType == "random")
                {
                    if (checkAttractor(null))
                        return loc.getRandomTile() * Game1.tileSize;
                    return null;
                }
                else if (AttractorType == "terrainfeature")
                {
                    var keys = loc.terrainFeatures.Keys.ToList();
                    keys.Shuffle();
                    foreach (var key in keys)
                    {
                        // if (IsType != null && IsType != "" && !o.GetType().IsInstanceOfType(Type.GetType(IsType)))
                        string featureName = loc.terrainFeatures[key].GetType().ToString().Split('.').Last().ToString();
                        BugNetMod.instance.Monitor.Log(featureName + " is featurename " + AttractorName + " is attractorname");
                        if (AttractorName != null && AttractorName != "" && AttractorName == featureName)
                        {
                            BugNetMod.instance.Monitor.Log("terrain feature" + loc.terrainFeatures[key].GetType().ToString());
                            if (checkAttractor(loc.terrainFeatures[key]))
                                return key * Game1.tileSize;
                        }
                    }

                    return null;
                }
                else if (AttractorType == "object")
                {
                    var keys = loc.objects.Keys.ToList();
                    keys.Shuffle();
                    foreach (var key in keys)
                    {
                        BugNetMod.instance.Monitor.Log(key.ToString() + "  location  " + loc.objects[key].displayName);

                        if (checkAttractor(loc.objects[key]))
                            return key * Game1.tileSize;
                    }

                    return null;
                }
                else throw new ArgumentException("Bad location type");
            }
            public bool checkAttractor(object obj)
            {
                BugNetMod.instance.Monitor.Log(obj.ToString() + "is obj");
                bool ret = true;

                //if (Chance != 1.0 && Game1.random.NextDouble() > Chance)
                //    ret = false;
                if (PropertyName != null && PropertyName != "")
                {
                    BugNetMod.instance.Monitor.Log("PropertyName: " + PropertyName);
                    string[] toks = PropertyName.Split('.');

                    var o = obj;
                    BugNetMod.instance.Monitor.Log(o.GetType().ToString());

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
                        BugNetMod.instance.Monitor.Log(o.ToString());
                    }

                    if (o != null)
                    {
                        if (IsType != null && IsType != "" && !o.GetType().IsInstanceOfType(Type.GetType(IsType)))
                            ret = false;
                        else if (IsValue != null && IsValue != "" && !o.ToString().Equals(IsValue))
                            ret = false;
                    }
                    //else if (RequireNotNull)
                    //    ret = false;
                    //} 
                }


                return ret;
            }

        }

        public Vector2? checkLocation(GameLocation location)
        {
            if (MinTimeOfDay != -1 && Game1.timeOfDay < MinTimeOfDay)
                return null;
            else if (MaxTimeOfDay != -1 && Game1.timeOfDay > MaxTimeOfDay)
                return null;
            else if (Seasons != null && Seasons.Count() > 0 && !Seasons.Contains(Game1.currentSeason))
                return null;
            else if (Locations != null && Locations.Count() > 0 && !Locations.Contains(location.Name))
                return null;
            else if (RequireDarkOut && !Game1.isDarkOut())
                return null;
            else if (!AllowRain && Game1.isRaining)
                return null;
            else if (Attractors.Count > 0)
            {
                foreach(Attractor attractor in Attractors)
                {
                    var spawnLocation = attractor.checkLocation(location);
                    if (spawnLocation != null)
                        return spawnLocation;
                }
            }
            return null;
        }
    }
}
