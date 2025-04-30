using BoneLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using UnityEngine;

using MelonLoader.Utils;
using NEP.ScoreLab.HUD;
using NEP.ScoreLab.Menu;

namespace NEP.ScoreLab.Data
{
    public static class HUDLoader
    {
        public static Dictionary<string, AudioClip> HUDAudioBank;
        public static Dictionary<string, GameObject> LoadedHUDs;
        public static List<JSONHUDManifest> LoadedHUDManifests;

        private static List<AssetBundle> _assetBundles;
        private static List<UnityEngine.Object> _persistentBundleObjects;
        
        public static void Initialize()
        {
            LoadedHUDs = new Dictionary<string, GameObject>();
            HUDAudioBank = new Dictionary<string, AudioClip>();
            LoadedHUDManifests = new List<JSONHUDManifest>();
            _persistentBundleObjects = new List<UnityEngine.Object>();
            _assetBundles = new List<AssetBundle>();
            
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
                    
                // This isn't necessary to work, but get any audio clips
                Il2CppReferenceArray<UnityEngine.Object> clipObjects = hudBundle.LoadAllAssets();
                
                // Optionally, retrieve the HUDs logo if it exists
                Texture2D hudLogo = hudBundle.LoadPersistentAsset<Texture2D>("Logo");

                if (hudLogo != null)
                {
                    hudManifest.SetHUDLogo(hudLogo);
                }
                
                _assetBundles.Add(hudBundle);
                
                LoadedHUDManifests.Add(hudManifest);
                LoadedHUDs.Add(hudManifest.Name, hudObject);

                if (clipObjects == null || clipObjects.Length == 0)
                {
                    continue;
                }
                
                foreach (var clip in clipObjects)
                {
                    AudioClip clipCasted = clip.TryCast<AudioClip>();
                    
                    if (clipCasted != null)
                    {
                        clipCasted.hideFlags = HideFlags.DontUnloadUnusedAsset;
                        HUDAudioBank.Add(clip.name, clipCasted);
                    }
                }
            }
        }

        public static void ReloadHUDs()
        {
            if (HUDManager.Instance.ActiveHUD != null)
            {
                HUDManager.Instance.DestroyHUD();
            }
            
            HUDManager.Instance.DestroyLoadedHUDs();

            HUDAudioBank.Clear();
            LoadedHUDManifests.Clear();
            LoadedHUDs.Clear();

            foreach (var assetBundle in _assetBundles)
            {
                assetBundle.Unload(true);
            }
            
            _assetBundles.Clear();
            
            LoadHUDs();
            
            SLMenu.RefreshHUDPage();
            
            HUDManager.Instance.Populate();
            HUDManager.Instance.LoadHUD(HUDManager.Instance.LastHUD.name);
        }
    }
}