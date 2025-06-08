using MelonLoader.Utils;
using NEP.ScoreLab.Core;
using UnityEngine;
using Newtonsoft.Json;

namespace NEP.ScoreLab.Data
{
    public static class ValueManager
    {
        public static Dictionary<string, ValuePackage> Packages { get; private set; }
        public static ValuePackage ActivePackage { get; private set; }
        public static Dictionary<string, int> HighScoreTable;
        
        public static readonly string Path_Developer      = Path.Combine(MelonEnvironment.UserDataDirectory, "Not Enough Photons");
        public static readonly string Path_Mod            = Path.Combine(Path_Developer, "ScoreLab");
        private static readonly string File_HighScores = Path.Combine(Path_Mod, "high_score_table.json");

        public static void Initialize()
        {
            HighScoreTable = new Dictionary<string, int>();
            Packages = new Dictionary<string, ValuePackage>();
            // HighScoreTable = ReadHighScore();
            foreach (var manifest in HUDLoader.LoadedHUDManifests)
            {
                JSONScore[] score = GetScoresForHUD(manifest.Name);
                JSONMult[] multipliers = GetMultipliersForHUD(manifest.Name);
                PackedScore[] scoreObjects = CreateScoreObjects(score);
                PackedMultiplier[] multiplierObjects = CreateMultiplierObjects(multipliers);
                Packages.Add(manifest.Name, new ValuePackage(scoreObjects, multiplierObjects));
            }

            UsePackage(Settings.SavedHUD);
        }

        public static string[] GetAllFiles(string path, string extensionFilter)
        {
            string[] files = Directory.GetFiles(path);
            List<string> filteredFiles = new List<string>();
        
            foreach (string file in files)
            {
                if (file.EndsWith(extensionFilter))
                {
                    filteredFiles.Add(file);
                }
            }
        
            return filteredFiles.ToArray();
        }
        
        public static void UsePackage(string name)
        {
            if (Packages.TryGetValue(name, out ValuePackage package))
            {
                ActivePackage = package;
            }
        }
        
        public static PackedValue Get(string eventType)
        {
            if (ActivePackage.Values == null)
            {
                return null;
            }
            
            if (ActivePackage.Values.TryGetValue(eventType, out PackedValue value))
            {
                return value;
            }

            return null;
        }

        public static Dictionary<string, int> ReadHighScore()
        {
            string directory = File_HighScores;
            return JsonConvert.DeserializeObject(directory) as Dictionary<string, int>;
        }

        public static void WriteHighScore()
        {
            string directory = File_HighScores;

            if (!File.Exists(directory))
            {
                Main.Logger.Warning("High score file doesn't exist! Creating one.");
                File.Create(directory);
                return;
            }

            var data = JsonConvert.SerializeObject(HighScoreTable);
            File.WriteAllText(directory, data);
        }

        public static void WriteBestScore(PackedHighScore highScore)
        {
            string sceneName = highScore.Name;
            int bestScore = highScore.bestScore;

            HighScoreTable.Add(sceneName, bestScore);
        }

        private static JSONScore[] GetScoresForHUD(string name)
        {
            string hudPath = Path_Mod;
            string dataPath = Path.Combine(hudPath, name + "/Data/Score");

            if (!Directory.Exists(dataPath))
            {
                Main.Logger.Warning($"HUD {name} doesn't have a score data folder!");
                return null;
            }
            
            string[] files = GetAllFiles(dataPath, ".json");
            
            List<JSONScore> scores = new List<JSONScore>();

            foreach (var file in files)
            {
                var data = ReadScoreData(file);
                scores.Add(data);
            }
            
            return scores.ToArray();
        }

        private static JSONMult[] GetMultipliersForHUD(string name)
        {
            string hudPath = Path_Mod;
            string dataPath = Path.Combine(hudPath, name + "/Data/Multiplier");
            
            if (!Directory.Exists(dataPath))
            {
                Main.Logger.Warning($"HUD {name} doesn't have a multiplier data folder!");
                return null;
            }
            
            string[] files = GetAllFiles(dataPath, ".json");
                        
            List<JSONMult> multipliers = new List<JSONMult>();
            
            foreach (var file in files)
            { 
                var data = ReadMultiplierData(file);
                multipliers.Add(data);
            }
                        
            return multipliers.ToArray();
        }
        
        private static PackedScore[] CreateScoreObjects(JSONScore[] scores)
        {
            if (scores == null)
            {
                return null;
            }
            
            List<PackedScore> scoreObjects = new List<PackedScore>();
            
            foreach (var score in scores)
            {
                var packedScore = new PackedScore(score);
                
                if (score.Tiers != null && score.Tiers.Length > 0)
                {
                    List<PackedScore> tiers = new List<PackedScore>();

                    foreach (var tier in score.Tiers)
                    {
                        var tierData = new PackedScore(tier, true);
                        tierData.eventType = packedScore.eventType;
                        
                        tiers.Add(tierData);
                    }

                    packedScore.Tiers = tiers.ToArray();
                }
                
                scoreObjects.Add(packedScore);
            }

            return scoreObjects.ToArray();
        }

        private static PackedMultiplier[] CreateMultiplierObjects(JSONMult[] multipliers)
        {
            if (multipliers == null)
            {
                return null;
            }
            
            List<PackedMultiplier> multObjects = new List<PackedMultiplier>();
            
            foreach (var multiplier in multipliers)
            {
                var packedMult = new PackedMultiplier(multiplier);
                
                if (multiplier.Tiers != null && multiplier.Tiers.Length > 0)
                {
                    List<PackedMultiplier> tiers = new List<PackedMultiplier>();

                    foreach (var tier in multiplier.Tiers)
                    {
                        var tierData = new PackedMultiplier(tier, true);
                        tierData.eventType = packedMult.eventType;
                        
                        tiers.Add(tierData);
                    }

                    packedMult.Tiers = tiers.ToArray();
                }

                multObjects.Add(packedMult);
            }

            return multObjects.ToArray();
        }

        private static JSONScore ReadScoreData(string file)
        {
            JSONScore data = new JSONScore();
            data.FromJSON(file);
            return data;
        }

        private static JSONMult ReadMultiplierData(string file)
        {
            JSONMult data = new JSONMult();
            data.FromJSON(file);
            return data;
        }
    }
}