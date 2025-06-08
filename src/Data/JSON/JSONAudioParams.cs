using UnityEngine;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NEP.ScoreLab.Data
{
    public struct JSONAudioParams
    {
        public string[] sounds;
        public float volume;
        public float pitch;
        
        public bool FromJSON(JObject jObject)
        {
            if (jObject == null)
            {
                return false;
            }
            
            JArray soundArray = jObject["Clips"] as JArray;

            if (soundArray != null)
            {
                sounds = new string[soundArray.Count];
                for (int i = 0; i < sounds.Length; i++)
                {
                    sounds[i] = soundArray[i].Value<string>();
                }
            }
            
            volume = jObject["Volume"].Value<float>();
            pitch = jObject["Pitch"].Value<float>();
            return true;
        }
    }
}