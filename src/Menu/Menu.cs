using UnityEngine;
using BoneLib.BoneMenu;
using NEP.ScoreLab.Core;
using NEP.ScoreLab.Data;
using NEP.ScoreLab.HUD;

namespace NEP.ScoreLab.Menu
{
    public static class SLMenu
    {
        public static void Initialize()
        {
            Page root = Page.Root.CreatePage("Not Enough Photons", Color.white);
            Page modPage = root.CreatePage("ScoreLab", Color.white);
            Page hudPage = modPage.CreatePage("HUDs", Color.white);

            for (int i = 0; i < HUDLoader.LoadedHUDManifests.Count; i++)
            {
                int index = i;
                var manifest = HUDLoader.LoadedHUDManifests[index];

                var function = hudPage.CreateFunction(manifest.Name, Color.white, () => HUDManager.Instance.LoadHUD(manifest.Name));
                function.Logo = manifest.Logo;
            }
        }
    }
}