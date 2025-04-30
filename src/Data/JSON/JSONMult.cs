using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NEP.ScoreLab.Data
{
    public struct JSONMult
    {
        public string EventType;
        public string TierEventType;

        public float DecayTime;
        public bool Stackable;

        public string Name;
        public float Multiplier;
        public string Condition;

        public int TierRequirement;

        public JSONMult[] Tiers;

        public bool FromJSON(string pathToJson)
        {
            using (StreamReader sr = new StreamReader(pathToJson))
            {
                using (JsonTextReader jsonReader = new JsonTextReader(sr))
                {
                    JObject data = JToken.ReadFrom(jsonReader) as JObject;

                    if (data == null)
                    {
                        return false;
                    }

                    ReadMultObject(data);
                }
            }

            return true;
        }

        private void ReadMultObject(JObject obj)
        {
            EventType = obj["EventType"].Value<string>();
            DecayTime = obj["DecayTime"].Value<float>();
            Stackable = obj["Stackable"].Value<bool>();
            Condition = obj["Condition"].Value<string>();

            Name = obj["Name"].Value<string>();
            Multiplier = obj["Multiplier"].Value<float>();
            TierRequirement = obj["TierRequirement"].Value<int>();

            JArray tierObjects = obj["Tiers"]?.Value<JArray>();
            List<JSONMult> tiers = new List<JSONMult>();

            if (tierObjects == null)
            {
                return;
            }

            for (int i = 0; i < tierObjects.Count; i++)
            {
                JSONMult tier = new JSONMult();
                JObject child = tierObjects[i] as JObject;
                tier.ReadMultObject(child);
                tiers.Add(tier);
            }

            Tiers = tiers.ToArray();
        }
    }
}