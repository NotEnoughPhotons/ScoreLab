using MelonLoader;
using NEP.NEDebug.Console;
using NEP.ScoreLab.HUD;

namespace NEP.ScoreLab.Core
{
    public static class Settings
    {
        public static float DistanceToCamera;
        public static float MovementSmoothness;
        public static bool UseAnnouncer;
        public static string SavedHUD;
        public static HUDShowMode HUDShowMode;

        private static MelonPreferences_Category m_scorelabCategory;
        private static MelonPreferences_Entry<float> m_eDistanceToCamera;
        private static MelonPreferences_Entry<float> m_eMovementSmoothness;
        private static MelonPreferences_Entry<bool> m_eUseAnnouncer;
        private static MelonPreferences_Entry<string> m_eSavedHUD;
        private static MelonPreferences_Entry<int> m_eHudShowMode;
        
        public static void CreatePreferences()
        {
            m_scorelabCategory = MelonPreferences.CreateCategory("ScoreLab");
            m_eDistanceToCamera = m_scorelabCategory.CreateEntry("DistanceToCamera", 1.125f);
            m_eMovementSmoothness = m_scorelabCategory.CreateEntry("MovementSmoothness", 16f);
            m_eUseAnnouncer = m_scorelabCategory.CreateEntry("UseAnnouncer", true);
            m_eSavedHUD = m_scorelabCategory.CreateEntry("SavedHUD", "Coda");
            m_eHudShowMode = m_scorelabCategory.CreateEntry("HUDShowMode", (int)HUDShowMode.Always);

            DistanceToCamera = m_eDistanceToCamera.Value;
            MovementSmoothness = m_eMovementSmoothness.Value;
            UseAnnouncer = m_eUseAnnouncer.Value;
            SavedHUD = m_eSavedHUD.Value;
            HUDShowMode = (HUDShowMode)m_eHudShowMode.Value;
        }
        
        public static void Save(bool log = false)
        {
            m_eDistanceToCamera.Value = DistanceToCamera;
            m_eMovementSmoothness.Value = MovementSmoothness;
            m_eUseAnnouncer.Value = UseAnnouncer;
            m_eSavedHUD.Value = SavedHUD;
            m_scorelabCategory.SaveToFile(log);
        }

        [NEConsoleCommand("scorelab.set_cam_dist")]
        public static void SetDistanceToCamera(float distance)
        {
            m_eDistanceToCamera.Value = distance;
        }

        [NEConsoleCommand("scorelab.set_movement_smoothness")]
        public static void SetMovementSmoothness(float movementSmoothness)
        {
            m_eMovementSmoothness.Value = movementSmoothness;
        }

        [NEConsoleCommand("scorelab.use_announcer")]
        public static void SetUseAnnouncer(bool useAnnouncer)
        {
            m_eUseAnnouncer.Value = useAnnouncer;
        }

        public static void SetHUDShowMode(HUDShowMode mode)
        {
            m_eHudShowMode.Value = (int)mode;
            HUDManager.HUDShowModeUpdated(mode);
        }
    }
}