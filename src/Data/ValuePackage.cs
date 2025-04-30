namespace NEP.ScoreLab.Data
{
    public struct ValuePackage
    {
        public ValuePackage(PackedScore[] scores, PackedMultiplier[] multipliers)
        {
            Values = new Dictionary<string, PackedValue>();
            
            foreach (var score in scores)
            {
                Values.Add(score.eventType, score);
            }

            foreach (var mult in multipliers)
            {
                Values.Add(mult.eventType, mult);
            }
        }

        public Dictionary<string, PackedValue> Values;
    }
}