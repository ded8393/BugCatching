using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.Xna.Framework;

using StardewValley;
using Critter = StardewValley.BellsAndWhistles.Critter;
using StardewValley.TerrainFeatures;

using StardewModdingAPI;
using StardewModdingAPI.Framework.ModHelpers;

using PyTK.CustomElementHandler;

using PyTK.Extensions;
using PyTK.Types;

namespace BugCatching
{
    public class CritterLocation
     {
        public string layerName { get; set; }
        public Vector2 tilePosition { get; set; } = new Vector2();
        public CritterEntry CritterEntry { get; set; } = new CritterEntry();
        public bool isDisturbableType { get; set; } = false;

        public CritterLocation(){ }
        public CritterLocation (CritterEntry critter)
        {
            this.CritterEntry = critter;
            if (CritterEntry.Behavior.DisturbableHome || CritterEntry.Classification.Equals("Digger"))
                isDisturbableType = true;
            if (CritterEntry.Classification == "Digger")
                layerName = "Back";
            else layerName = "Front";
              
        }
        public bool attemptSpawn()
        {
            bool spawn = true;
            if (!CritterEntry.SpawnConditions.allConditionsMet())
            {
                spawn = false;
                Log.debug($"{CritterEntry.BugModel.Name} -- not all spawn conditions met");
            }
            if (CritterEntry.BugModel.Rarity != 1.0 && CritterEntry.BugModel.Rarity < Game1.random.NextDouble())
            {
                spawn = false;
                Log.debug($"{CritterEntry.BugModel.Name} -- rarity spawn failure");

            }

            return spawn;
        }
    }
    public class CritterLocations
    {
        public GameLocation Location = new GameLocation();
        public List<CritterEntry> LocalSeasonalCritters = new List<CritterEntry>();
        public Dictionary<string, List<Vector2>> AllPossibleHomesByBug = new Dictionary<string, List<Vector2>>();
        public List<CritterLocation> CritterHomes = new List<CritterLocation>();
        public List<CritterLocation> ActiveCritters = new List<CritterLocation>();
        public List<CritterLocation> DisturbableCritters = new List<CritterLocation>();

        public int CritterCount;
        public List<Critter> critterList;

        public CritterLocations(GameLocation gameLocation)
        {
            Location = Game1.getLocationFromName(gameLocation.Name);
            //these should really  be globally registered

            //todo check for return null
        }

        public void updateActiveCritters(List<Critter> UpdatedCritterList)
        {
            this.Location.GetType().GetField("critters", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(this.Location, UpdatedCritterList);
        }

        public void removeThisCritter(Critter critter)
        {
            List<Critter> newCritterList = getAllActiveCritters();
            newCritterList.Remove(critter);
            updateActiveCritters(newCritterList);
        }

        public void registerCritterHome(CritterLocation critterLocation)
        {
            Log.info($"Registering critterLocation {Location} {critterLocation.layerName} {critterLocation.tilePosition} {critterLocation.CritterEntry}");
            CritterHomes.AddOrReplace(critterLocation);
        }

        public void activateCritter(CritterLocation critterLocationEntry)
        {
            if (critterLocationEntry == null)
            {
                Log.error("cannot activate; null critterLocationEntry");
                return;
            }

            CritterEntry critterEntry = CritterEntry.critters.Find(ce => ce.Value == critterLocationEntry.CritterEntry).Value;
            if (critterEntry == null)
            {
                Log.error("cannot activate; null critterEntry");
                return;
            }
            Location.addCritter(critterEntry.makeCritter(critterLocationEntry.tilePosition));
            CritterHomes.Remove(critterLocationEntry);
        }

        public void deactivateCritter(Critter critter)
        {
            removeThisCritter(critter);
            
        }

        public void spawnCritters()
        {
            Log.info($"there are ({CritterHomes.Count}) Critter Homes at this location");
            List<CritterLocation> crittersToSpawn = new List<CritterLocation>();
            foreach(CritterLocation critterLocation in CritterHomes)
            {
                if (critterLocation.attemptSpawn())
                    crittersToSpawn.Add(critterLocation);
                else Log.debug($"{critterLocation.CritterEntry.BugModel.Name} is at Home");

            }
            foreach (CritterLocation spawnLocation in crittersToSpawn)
            {
                if (spawnLocation.isDisturbableType)
                    addDisturbableHome(spawnLocation);
                else activateCritter(spawnLocation);
            }

            
        }

        public List<Critter> getAllActiveCritters()
        {
            if (!Location.IsOutdoors)
                return new List<Critter>();
            object privateFieldValue = this.Location.GetType().GetField("critters", BindingFlags.NonPublic | BindingFlags.Instance)
                            .GetValue(this.Location);
            return (List<Critter>)privateFieldValue;
        }

        public CritterLocation getCritterHome(string layerName, Vector2 tile)
        {
            CritterLocation locationEntry = new CritterLocation();
            locationEntry = CritterHomes.Find(c => c.layerName == layerName && c.tilePosition == tile);
            return locationEntry;
        }


        public void buildAllPossibleHomes()
        {
            if (CritterHomes.Count > 0)
                CritterHomes.Clear();

            LocalSeasonalCritters = BugCatchingMod.SeasonalCritters.Where(c => c.SpawnConditions.Locations.Contains(Location.Name)).ToList();
            Log.info($"in Location {Location} there are ({LocalSeasonalCritters.Count}/{BugCatchingMod.SeasonalCritters.Count}) local seasonal critters");
            foreach (CritterEntry critterEntry in LocalSeasonalCritters)
            {
                Log.info($"building AllHomes for {critterEntry.BugModel.Name}");
                string critterId = CritterEntry.GetCritterIdFromRegistry(critterEntry);
                List<Vector2> possibleSpawnLocations = new List<Vector2>();

                foreach (Attractor attractor in critterEntry.Attractors)
                {
                    List<Vector2> attractorTiles = new List<Vector2>();
                    attractorTiles = attractor.getAttractorTilesAtLocation(Location);
                    Log.info($"{attractorTiles.Count} here");
                    possibleSpawnLocations.AddRange(attractorTiles);
                }
                Log.info($"found ({possibleSpawnLocations.Count}) Suitable Homes for {critterEntry.BugModel.Name}");
                AllPossibleHomesByBug.AddOrReplace(critterId, possibleSpawnLocations);               
            }
        }

        public void balanceSpawning()
        {
            int numBugs = AllPossibleHomesByBug.Keys.Count;
            double sumRarity = BugCatchingMod.AllBugs.Where(bug => AllPossibleHomesByBug.ContainsKey(bug.Id)).Aggregate(0.0, (acc, bug) => acc + (1f - bug.Rarity));
            int nTotalBugs = (int)Math.Floor(numBugs * sumRarity);
            Log.info($"location has ({nTotalBugs}) total bugs with a sumRarity of ({sumRarity})");

            foreach (KeyValuePair<string, List<Vector2>> kvp in AllPossibleHomesByBug)
            {
                CritterEntry critterEntry = CritterEntry.critters[kvp.Key];
                List<Vector2> possibleSpawnLocations = new List<Vector2>(); 
                possibleSpawnLocations = kvp.Value;
                int nCritterSpawned = (int)Math.Ceiling(critterEntry.BugModel.Rarity * nTotalBugs);
                Log.info($"({nCritterSpawned}) {critterEntry.BugModel.Name} will spawn");
                if (possibleSpawnLocations.Count > 0)
                {
                    for (int i = 0; i <= nCritterSpawned; i++)
                    {
                        Vector2 tile = possibleSpawnLocations.PopRandom();
                        Log.info($"Adding {critterEntry.BugModel.Name} at location {tile}");
                        CritterLocation critterLocation = new CritterLocation(critterEntry) { tilePosition = tile };
                        registerCritterHome(critterLocation);

                    }
                }
                else Log.error($"{critterEntry.BugModel.Name} lacks Suitable Housing");
            }
        }
    
        public void addDisturbableHome(CritterLocation critterLocationEntry)
        {
            if (critterLocationEntry.CritterEntry.Classification == "Digger")
                Location.setTileProperty((int)critterLocationEntry.tilePosition.X, (int)critterLocationEntry.tilePosition.Y, "Back", "Diggable", "T");
            else if (critterLocationEntry.CritterEntry.Behavior.DisturbableHome)
                this.Location.Map.addTouchAction(critterLocationEntry.tilePosition, "disturbBug", "");

        }

    }

}
