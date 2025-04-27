using BoneLib;
using UnityEngine;

using MelonLoader.Utils;

namespace NEP.ScoreLab.Data
{
    public static class HUDLoader
    {
        public static Dictionary<string, GameObject> LoadedHUDs;
        public static List<JSONHUDManifest> LoadedHUDManifests;

        public static void Initalize()
        {
            LoadedHUDs = new Dictionary<string, GameObject>();
            LoadedHUDManifests = new List<JSONHUDManifest>();
            
            LoadHUDs();
        }

        public static void SpawnDefaultUI()
        {
            SpawnUI("Coda");
        }

        public static void SpawnUI(string name)
        {
            foreach (var manifest in LoadedHUDManifests)
            {
                if (manifest.Name == name)
                {
                    SpawnUI(LoadedHUDs[name]);
                }
            }
        }

        public static void SpawnUI(GameObject uiObject)
        {
            GameObject.Instantiate(uiObject);
        }
        
        public static void LoadHUDs()
        {
            string modPath = Path.Combine(MelonEnvironment.UserDataDirectory, "Not Enough Photons/ScoreLab");
            string hudPath = Path.Combine(modPath, "HUDs");
            
            var folders = Directory.EnumerateDirectories(hudPath);

            foreach (var folder in folders)
            {
                // Go inside the folder and get all files
                var files = Directory.EnumerateFiles(folder);
                string hud = string.Empty;
                string manifest = string.Empty;

                // Check file extension for the bundle and manifest
                foreach (var file in files)
                {
                    if (file.EndsWith(".hud"))
                    {
                        hud = file;
                    }

                    if (file.EndsWith(".hud_manifest"))
                    {
                        manifest = file;
                        break;
                    }
                }

                if (hud == string.Empty)
                {
                    Main.Logger.Error($"Missing .HUD file in {folder}!");
                    continue;
                }
                else if (manifest == string.Empty)
                {
                    Main.Logger.Error($"Missing HUD manifest file in {folder}!");
                    continue;
                }
                
                // Load the manifest
                JSONHUDManifest hudManifest = new JSONHUDManifest();
                if (!hudManifest.FromJSON(manifest))
                {
                    Main.Logger.Error($"Failed to read HUD manifest data! Source: {folder}");
                    continue;
                }

                // Load the HUD bundle
                AssetBundle hudBundle = AssetBundle.LoadFromFile(hud);
                    
                // Check if the bundle has a valid HUD object
                GameObject hudObject = hudBundle.LoadPersistentAsset<GameObject>($"[SLHUD] - {hudManifest.Name}");
                    
                // Optionally, retrieve the HUDs logo if it exists
                Texture2D hudLogo = hudBundle.LoadPersistentAsset<Texture2D>("Logo");

                if (hudLogo != null)
                {
                    hudManifest.SetHUDLogo(hudLogo);
                }
                
                LoadedHUDManifests.Add(hudManifest);
                LoadedHUDs.Add(hudManifest.Name, hudObject);
            }
        }
    }
}