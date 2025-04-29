using MelonLoader.Utils;
using Newtonsoft.Json;

using UnityEngine;

namespace NEP.ScoreLab.Data
{
    public static class DataManager
    {
        public static readonly string Path_Developer      = Path.Combine(MelonEnvironment.UserDataDirectory, "Not Enough Photons");
        public static readonly string Path_Mod            = Path.Combine(Path_Developer, "ScoreLab");
        public static readonly string Path_CustomUIs      = Path.Combine(Path_Mod, "HUDs");
        public static readonly string Path_SFX            = Path.Combine(Path_Mod, "SFX");

        static readonly string File_HUDSettings    = Path.Combine(Path_Mod, "sl_hud_settings.json");
        static readonly string File_CurrentHUD     = Path.Combine(Path_Mod, "sl_current_hud.txt");

        public static void Initialize()
        {
            InitializeDirectories();
        }

        public static string[] GetAllFiles(string path, string extensionFilter)
        {
            string[] files = Directory.GetFiles(path);
            List<string> filteredFiles = new List<string>();

            foreach (string file in files)
            {
                if (file.EndsWith(extensionFilter))
                {
                    filteredFiles.Add(file);
                }
            }

            return filteredFiles.ToArray();
        }

        private static void InitializeDirectories()
        {
            Directory.CreateDirectory(Path_Mod);
            Directory.CreateDirectory(Path_CustomUIs);
            Directory.CreateDirectory(Path_SFX);
        }
    }
}

