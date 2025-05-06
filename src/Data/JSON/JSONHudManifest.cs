using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NEP.ScoreLab.Data
{
    public struct JSONHUDManifest
    {
        public string Name;
        public string Author;
        public string Description;
        public string[] Tags;
        public string AssetName;
        public Texture2D Logo;

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
                    
                    Name = data["name"].Value<string>();
                    Author = data["author"].Value<string>();
                    AssetName = data["assetName"].Value<string>();
                }
            }

            return true;
        }

        public void SetHUDLogo(Texture2D texture)
        {
            Logo = texture;
        }
    }
}