using MelonLoader;

using UnityEngine;

using NEP.ScoreLab.Core;
using NEP.ScoreLab.Data;
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
        public override void OnLateInitializeMelon()
        {
            Hooks.Initialize();

            Hooks.Game.OnMarrowGameStarted += OnMarrowGameStarted;
            Hooks.Game.OnMarrowSceneLoaded += OnMarrowSceneLoaded;
            
            SLMenu.Initialize();
        }

        public void OnMarrowGameStarted()
        {
            DataManager.Init();
            ScoreDirector.Patches.InitPatches();
            new ScoreTracker().Initialize();
        }

        public void OnMarrowSceneLoaded(MarrowSceneInfo sceneInfo)
        {
            new GameObject("[ScoreLab] - UI Manager").AddComponent<UI.UIManager>();
            new GameObject("[ScoreLab] - Audio Manager").AddComponent<Audio.AudioManager>();
            
            // TODO: Add high scores, and add an option to reset level progress
            ScoreTracker.Instance.ResetScore();
            ScoreTracker.Instance.ResetMultiplier();
        }

        public override void OnUpdate()
        {
            if(ScoreTracker.Instance == null)
            {
                return;
            }

            ScoreTracker.Instance.Update();
        }
    }
}