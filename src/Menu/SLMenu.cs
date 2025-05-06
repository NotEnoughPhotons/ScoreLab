using UnityEngine;
using BoneLib.BoneMenu;
using NEP.ScoreLab.Core;
using NEP.ScoreLab.Data;
using NEP.ScoreLab.HUD;

namespace NEP.ScoreLab.Menu
{
    public static class SLMenu
    {
        private static Page _hudPage;
        
        public static void Initialize()
        {
            Page root = Page.Root.CreatePage("Not Enough Photons", Color.white);
            Page modPage = root.CreatePage("ScoreLab", Color.white);
            _hudPage = modPage.CreatePage("HUDs", Color.white);

            #if DEBUG
            modPage.CreateFunction("Reload HUDs", Color.white, () => HUDLoader.ReloadHUDs());
            #endif
            modPage.CreateFloat("HUD Distance", Color.white, 1.125f, 0.025f, 0f, 2f,
                (value) => Settings.DistanceToCamera = value);
            
            for (int i = 0; i < HUDLoader.LoadedHUDManifests.Count; i++)
            {
                int index = i;
                var manifest = HUDLoader.LoadedHUDManifests[index];

                var function = _hudPage.CreateFunction(manifest.Name, Color.white, () => HUDManager.LoadHUD(manifest.Name));
                function.Logo = manifest.Logo;
            }
        }

        public static void RefreshHUDPage()
        {
            _hudPage.RemoveAll();
            
            for (int i = 0; i < HUDLoader.LoadedHUDManifests.Count; i++)
            {
                int index = i;
                var manifest = HUDLoader.LoadedHUDManifests[index];

                var function = _hudPage.CreateFunction(manifest.Name, Color.white, () => HUDManager.LoadHUD(manifest.Name));
                function.Logo = manifest.Logo;
            }
        }
    }
}