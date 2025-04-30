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

            modPage.CreateFunction("Reload HUDs", Color.white, () => HUDLoader.ReloadHUDs());
            
            for (int i = 0; i < HUDLoader.LoadedHUDManifests.Count; i++)
            {
                int index = i;
                var manifest = HUDLoader.LoadedHUDManifests[index];

                var function = _hudPage.CreateFunction(manifest.Name, Color.white, () => SelectHUD(manifest.Name));
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

                var function = _hudPage.CreateFunction(manifest.Name, Color.white, () => SelectHUD(manifest.Name));
                function.Logo = manifest.Logo;
            }
        }

        private static void SelectHUD(string name)
        {
            ValueManager.UsePackage(name);
            HUDManager.Instance.LoadHUD(name);
        }
    }
}