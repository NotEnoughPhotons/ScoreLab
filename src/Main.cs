using MelonLoader;
using NEP.ScoreLab.Audio;
using UnityEngine;

using NEP.ScoreLab.Core;
using NEP.ScoreLab.Data;
using NEP.ScoreLab.HUD;
using NEP.ScoreLab.Patches;
using NEP.ScoreLab.Menu;

[assembly: MelonInfo(typeof(NEP.ScoreLab.Main), "ScoreLab", "1.0.0", "Not Enough Photons")]
[assembly: MelonGame("Stress Level Zero", "BONELAB")]

namespace NEP.ScoreLab
{
    public static class BuildInfo
    {
        public const string Name = "ScoreLab"; // Name of the Mod.  (MUST BE SET)
        public const string Author = "Not Enough Photons"; // Author of the Mod.  (Set as null if none)
        public const string Company = "Not Enough Photons"; // Company that made the Mod.  (Set as null if none)
        public const string Version = "1.0.0"; // Version of the Mod.  (MUST BE SET)
        public const string DownloadLink = "https://thunderstore.io/c/bonelab/p/NotEnoughPhotons/ScoreLab"; // Download Link for the Mod.  (Set as null if none)
    }

    public class Main : MelonMod
    {
        internal static MelonLogger.Instance Logger;
        
        public override void OnLateInitializeMelon()
        {
            Logger = new MelonLogger.Instance("ScoreLab");
            
            Hooks.Initialize();

            Hooks.Game.OnMarrowGameStarted += OnMarrowGameStarted;
            Hooks.Game.OnMarrowSceneLoaded += OnMarrowSceneLoaded;
            
            HUDLoader.Initialize();
            SLMenu.Initialize();
        }

        public void OnMarrowGameStarted()
        {
            ScoreTracker.Initialize();
            DataManager.Initialize();
            ValueManager.Initialize();
            AudioManager.Initialize();
            ScoreDirector.Patches.InitPatches();
        }

        public void OnMarrowSceneLoaded(MarrowSceneInfo sceneInfo)
        {
            new GameObject("[ScoreLab] - HUD Manager").AddComponent<HUDManager>();
            
            // TODO: Add high scores, and add an option to reset level progress
            ScoreTracker.ResetScore();
            ScoreTracker.ResetMultiplier();
        }

        public override void OnUpdate()
        {
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.F5))
            {
                HUDLoader.ReloadHUDs();
            }
            
            ScoreTracker.Update();
        }

        public override void OnDeinitializeMelon()
        {
            AudioManager.Uninitialize();
        }
    }
}