using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NEP.ScoreLab.Data
{
    public struct JSONAnnouncerManifest
    {
        public string[] Clips;

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
                    
                    JArray clips = data["clips"].ToObject<JArray>();

                    if (clips == null)
                    {
                        return false;
                    }

                    for (int i = 0; i < clips.Count; i++)
                    {
                        Clips[i] = clips[i].ToString();
                    }
                }
            }

            return true;
        }
    }
}