using UnityEngine;
using BoneLib.BoneMenu;
using NEP.ScoreLab.Core;

namespace NEP.ScoreLab.Menu
{
    public static class SLMenu
    {
        public static void Initialize()
        {
            Page nepPage = Page.Root.CreatePage("Not Enough Photons", Color.white, maxElements: 8, createLink: true);
            Page scoreLabPage = nepPage.CreatePage("ScoreLab", Color.white, maxElements: 0, createLink: true);

            Page settingsPage = scoreLabPage.CreatePage("Settings", Color.white, createLink: true);

            settingsPage.CreateFloat("Distance", Color.white, 1.25f, 0.25f, 0f, 5f,
                (value) => Settings.DistanceToCamera = value);
            settingsPage.CreateFloat("Smoothness", Color.white, 8f, 1f, 0f, 16f,
                (value) => Settings.MovementSmoothness = value);
            settingsPage.CreateBool("Use Announcer", Color.white, true, (value) => Settings.UseAnnouncer = value);
        }
    }
}