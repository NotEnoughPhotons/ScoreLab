using UnityEngine;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NEP.ScoreLab.Data
{
    public struct JSONAudioParams
    {
        public string sound;
        public float volume;
        public float pitch;
        
        public bool FromJSON(JObject jObject)
        {
            if (jObject == null)
            {
                return false;
            }
            
            sound = jObject["Clip"].Value<string>();
            volume = jObject["Volume"].Value<float>();
            pitch = jObject["Pitch"].Value<float>();
            return true;
        }
    }
}