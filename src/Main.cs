﻿using MelonLoader;
using NEP.ScoreLab.Audio;
using UnityEngine;

using NEP.ScoreLab.Core;
using NEP.ScoreLab.Data;
using NEP.ScoreLab.HUD;
using NEP.ScoreLab.Patches;
using NEP.ScoreLab.Menu;
using EventType = NEP.ScoreLab.Data.EventType;

[assembly: MelonInfo(typeof(NEP.ScoreLab.Main), "ScoreLab", "1.0.0", "Not Enough Photons")]
[assembly: MelonGame("Stress Level Zero", "BONELAB")]

namespace NEP.ScoreLab
{
    public static class BuildInfo
    {
        public const string Name = "ScoreLab";
        public const string Author = "Not Enough Photons";
        public const string Company = "Not Enough Photons";
        public const string Version = "1.0.0";
        public const string DownloadLink = "https://thunderstore.io/c/bonelab/p/NotEnoughPhotons/ScoreLab";
    }

    public class Main : MelonMod
    {
        internal static MelonLogger.Instance Logger;
        
        public override void OnLateInitializeMelon()
        {
            Logger = new MelonLogger.Instance("ScoreLab");
            
            Settings.CreatePreferences();
            
            Hooks.Initialize();

            Hooks.Game.OnMarrowGameStarted += OnMarrowGameStarted;
            Hooks.Game.OnMarrowSceneLoaded += OnMarrowSceneLoaded;
            
            HUDLoader.Initialize();
            SLMenu.Initialize();
        }

        public void OnMarrowGameStarted()
        {
            ScoreTracker.Initialize();
            ValueManager.Initialize();
            AudioManager.Initialize();
            ScoreDirector.Patches.InitPatches();
        }

        public void OnMarrowSceneLoaded(MarrowSceneInfo sceneInfo)
        {
            HUDManager.Initialize();
            
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
            HUDManager.Uninitialize();
            AudioManager.Uninitialize();
            Settings.Save();
        }
    }
}