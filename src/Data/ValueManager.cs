using UnityEngine;
using Newtonsoft.Json;

namespace NEP.ScoreLab.Data
{
    public static class ValueManager
    {
        public static Dictionary<string, PackedValue> ValueTable { get; private set; }
        public static Dictionary<string, int> HighScoreTable;
        public static JSONScore[] Scores { get; private set; }
        public static JSONMult[] Multipliers { get; private set; }

        private static string[] _scoreFiles;
        private static string[] _multiplierFiles;

        private static readonly string Path_ScoreData = Path.Combine(DataManager.Path_Mod, "Data/Score");
        private static readonly string Path_MultiplierData = Path.Combine(DataManager.Path_Mod, "Data/Multiplier");
        private static readonly string Path_HighScoreData = Path.Combine(DataManager.Path_Mod, "Data/High Score");

        private static readonly string File_HighScores = Path.Combine(Path_HighScoreData, "high_score_table.json");

        public static void Initialize()
        {
            Directory.CreateDirectory(Path_ScoreData);
            Directory.CreateDirectory(Path_MultiplierData);

            ValueTable = new Dictionary<string, PackedValue>();
            Scores = GetScores();
            Multipliers = GetMultipliers();
            HighScoreTable = new Dictionary<string, int>();
            // HighScoreTable = ReadHighScore();
            CreateScoreObjects();
            CreateMultiplierObjects();
        }

        public static PackedValue Get(string eventType)
        {
            return ValueTable[eventType];
        }

        public static PackedScore GetScore(string eventType)
        {
            PackedScore value = (PackedScore)Get(eventType);
            PackedScore score = new PackedScore()
            {
                eventType = value.eventType,
                Stackable = value.Stackable,
                EventAudio = value.EventAudio,
                Name = value.Name,
                Score = value.Score,
                AccumulatedScore = value.Score,
                DecayTime = value.DecayTime,
                TierRequirement = value.TierRequirement,
                Tiers = value.Tiers
            };

            return score;
        }

        public static PackedMultiplier GetMultiplier(string eventType)
        {
            PackedMultiplier value = (PackedMultiplier)Get(eventType);
            PackedMultiplier mult = new PackedMultiplier()
            {
                eventType = value.eventType,
                Stackable = value.Stackable,
                EventAudio = value.EventAudio,
                Name = value.Name,
                Multiplier = value.Multiplier,
                DecayTime = value.DecayTime,
                Condition = value.Condition,
                TierRequirement = value.TierRequirement,
                Tiers = value.Tiers
            };

            return mult;
        }

        public static Dictionary<string, int> ReadHighScore()
        {
            string directory = File_HighScores;

            if (!Directory.Exists(Path_HighScoreData))
            {
                Debug.LogWarning("High score file doesn't exist! Creating one.");
                Directory.CreateDirectory(Path_HighScoreData);
                return null;
            }

            return JsonConvert.DeserializeObject(directory) as Dictionary<string, int>;
        }

        public static void WriteHighScore()
        {
            string directory = File_HighScores;

            if (!File.Exists(directory))
            {
                Debug.LogWarning("High score file doesn't exist! Creating one.");
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

        private static JSONScore[] GetScores()
        {
            _scoreFiles = DataManager.GetAllFiles(Path_ScoreData, ".json");
            List<JSONScore> scores = new List<JSONScore>();

            foreach (var file in _scoreFiles)
            {
                var data = ReadScoreData(file);
                scores.Add(data);
            }

            return scores.ToArray();
        }

        private static JSONMult[] GetMultipliers()
        {
            _multiplierFiles = DataManager.GetAllFiles(Path_MultiplierData, ".json");
            List<JSONMult> multipliers = new List<JSONMult>();

            foreach (var file in _multiplierFiles)
            {
                var data = ReadMultiplierData(file);
                multipliers.Add(data);
            }

            return multipliers.ToArray();
        }

        private static void CreateScoreObjects()
        {
            foreach (var score in Scores)
            {
                var data = new PackedScore(score);
                
                if (score.Tiers != null && score.Tiers.Length > 0)
                {
                    List<PackedScore> tiers = new List<PackedScore>();

                    foreach (var tier in score.Tiers)
                    {
                        var tierData = new PackedScore(tier, true);
                        tierData.eventType = data.eventType;

                        if (tier.EventAudio != null)
                        {
                            AudioClip clip = DataManager.Audio.GetClip(tier.EventAudio);
                            tierData.EventAudio = clip;
                        }

                        tiers.Add(tierData);
                    }

                    data.Tiers = tiers.ToArray();
                }

                ValueTable.Add(score.EventType, data);
            }
        }

        private static void CreateMultiplierObjects()
        {
            foreach (var mult in Multipliers)
            {
                var data = new PackedMultiplier(mult);

                if (mult.Tiers != null && mult.Tiers.Length > 0)
                {
                    List<PackedMultiplier> tiers = new List<PackedMultiplier>();

                    foreach (var tier in mult.Tiers)
                    {
                        var tierData = new PackedMultiplier(tier, true);
                        tierData.eventType = data.eventType;

                        tiers.Add(tierData);
                    }

                    data.Tiers = tiers.ToArray();
                }

                ValueTable.Add(mult.EventType, data);
            }
        }

        private static JSONScore ReadScoreData(string file)
        {
            var data = File.ReadAllText(file);
            return JsonConvert.DeserializeObject<JSONScore>(data);
        }

        private static JSONMult ReadMultiplierData(string file)
        {
            var data = File.ReadAllText(file);
            return JsonConvert.DeserializeObject<JSONMult>(data);
        }
    }
}