using NEP.ScoreLab.Data;
using NEP.ScoreLab.HUD;

namespace NEP.ScoreLab.Core
{
    public static class API
    {
        public static class Value
        {
            public static Action<PackedValue> OnValueAdded;
            public static Action<PackedValue> OnValueAccumulated;
            public static Action<PackedValue> OnValueTierReached;
            public static Action<PackedValue> OnValueRemoved;
        }

        public static class Score
        {
            public static Action<int> OnScoreUpdated;
            public static Action<int> OnScoreDifferenceUpdated;
            public static Action<int> OnLastScoreUpdated;
        }

        public static class Multiplier
        {
            public static Action<PackedMultiplier> OnMultiplierTimeBegin;
            public static Action<PackedMultiplier> OnMultiplierTimeExpired;
        }

        public static class HighScore
        {
            public static Action<PackedHighScore> OnHighScoreReached;
            public static Action<PackedHighScore> OnHighScoreUpdated;
            public static Action<PackedHighScore> OnHighScoreLoaded;
            public static Action<PackedHighScore> OnHighScoreSaved;
        }

        public static class GameConditions
        {
            private static Dictionary<string, Func<bool>> _conditionTable = new Dictionary<string, Func<bool>>()
            {
                { "IsPlayerMoving", new Func<bool>(() => ScoreDirector.IsPlayerMoving) },
                { "IsPlayerSeated", new Func<bool>(() => ScoreDirector.IsPlayerSeated) },
                { "IsPlayerInAir", new Func<bool>(() => ScoreDirector.IsPlayerInAir) },
                { "IsPlayerRagdolled", new Func<bool>(() => ScoreDirector.IsPlayerRagdolled) },
                { "IsSlowMoEnabled", new Func<bool>(() => ScoreDirector.IsSlowMoEnabled) }
            };

            public static Func<bool> GetCondition(string cond)
            {
                if (string.IsNullOrEmpty(cond))
                {
                    UnityEngine.Debug.LogWarning($"{cond} doesn't exist in the condition table.");
                    return null;
                }
                else if (!_conditionTable.ContainsKey(cond))
                {
                    UnityEngine.Debug.LogWarning($"{cond} doesn't exist in the condition table.");
                    return null;
                }
                return _conditionTable[cond];
            }
        }

        public static class UI
        {
            public static Action<Module> OnModuleEnabled;
            public static Action<Module> OnModuleDisabled;

            public static Action<Module> OnModuleDecayed;
            public static Action<Module> OnModulePostDecayed;
        }
    }
}

