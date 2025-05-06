namespace NEP.ScoreLab.Data
{
    public static class EventType
    {
        public static readonly string None = "None";

        public static class Score
        {
            public static readonly string Kill = "SCORE_KILL";
            public static readonly string Headshot = "SCORE_HEADSHOT";
            public static readonly string EnemyMidAirKill = "SCORE_KILL_IN_AIR";
            public static readonly string GameWaveCompleted = "SCORE_WAVE_COMPLETED";
            public static readonly string GameRoundCompleted = "SCORE_ROUND_COMPLETED";
            public static readonly string Crabcest = "SCORE_CRABCEST";
            public static readonly string Facehug = "SCORE_FACEHUG";
        }

        public static class Mult
        {
            public static readonly string Kill = "MULT_KILL";
            public static readonly string MidAir = "MULT_MIDAIR";
            public static readonly string Seated = "MULT_SEATED";
            public static readonly string SecondWind = "MULT_SECONDWIND";
            public static readonly string Ragolled = "MULT_RAGDOLLED";
            public static readonly string SwappedAvatars = "MULT_SWAPPED_AVATARS";
            public static readonly string SlowMo = "MULT_SLOWMO";
        }
    }
}