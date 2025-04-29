using UnityEngine;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NEP.ScoreLab.Data
{
    public struct JSONAudioParams
    {
        public AudioClip sound;
        public float volume;
        public float pitch;
        
        public bool FromJSON(string pathToJson)
        {
            // Check the bank for a loaded sound
            using (StreamReader sr = new StreamReader(pathToJson))
            {
                using (JsonTextReader jsonReader = new JsonTextReader(sr))
                {
                    JObject data = JToken.ReadFrom(jsonReader) as JObject;
                    JArray array = data["clips"] as JArray;

                    if (data == null || array == null)
                    {
                        return false;
                    }

                    for (int i = 0; i < array.Count; i++)
                    {
                        string clipName = array[i].Value<string>();
                        sound = 
                    }
                }
            }

            return true;
        }
    }
}