using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NEP.ScoreLab.Data
{
    public struct JSONScore
    {
        public string EventType;
        public string TierEventType;

        public float DecayTime;
        public bool Stackable;
        public JSONAudioParams EventAudio;

        public string Name;
        public int Score;

        public int TierRequirement;

        public JSONScore[] Tiers;

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

                    ReadScoreObject(data);
                }
            }

            return true;
        }

        private void ReadScoreObject(JObject obj)
        {
            EventType = obj["EventType"].Value<string>();
            DecayTime = obj["DecayTime"].Value<float>();
            Stackable = obj["Stackable"].Value<bool>();
                    
            EventAudio.FromJSON(obj["EventAudio"].Value<JObject>());
                    
            Name = obj["Name"].Value<string>();
            Score = obj["Score"].Value<int>();
            TierRequirement = obj["TierRequirement"].Value<int>();
                    
            JArray tierObjects = obj["Tiers"]?.Value<JArray>();
            List<JSONScore> tiers = new List<JSONScore>();

            if (tierObjects == null)
            {
                return;
            }
            
            for (int i = 0; i < tierObjects.Count; i++)
            {
                JSONScore tier = new JSONScore();
                JObject child = tierObjects[i] as JObject;
                tier.ReadScoreObject(child);
                tiers.Add(tier);
            }
            
            Tiers = tiers.ToArray();
        }
    }
}

