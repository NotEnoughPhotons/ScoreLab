using UnityEngine;

using NEP.ScoreLab.Data;

namespace NEP.ScoreLab.HUD
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class HUDManager : MonoBehaviour
    {
        public HUDManager(System.IntPtr ptr) : base(ptr) { }

        public static HUDManager Instance { get; private set; }
        
        public List<HUD> LoadedHUDs { get; private set; }
        public HUD ActiveHUD { get; private set; }

        private void Awake()
        {
            Instance = this;
            
            LoadedHUDs = new List<HUD>();

            for(int i = 0; i < HUDLoader.LoadedHUDManifests.Count; i++)
            {
                var manifest = HUDLoader.LoadedHUDManifests[i];
                var hudObject = GameObject.Instantiate(HUDLoader.LoadedHUDs[manifest.Name]);
                hudObject.name = manifest.Name;

                var hud = hudObject.GetComponent<HUD>();

                // Not a valid UI
                if(hud == null)
                {
                    continue;
                }

                LoadedHUDs.Add(hud);
                hud.SetParent(transform);
                hud.gameObject.SetActive(false);
            }
        }

        private void Start()
        {
            LoadHUD("Coda");
        }

        public void LoadHUD(string name)
        {
            UnloadHUD();
            foreach (var hud in LoadedHUDs)
            {
                if (hud.name == name)
                {
                    ActiveHUD = hud;
                    break;
                }
            }

            if (ActiveHUD == null)
            {
                return;
            }
            
            ActiveHUD.gameObject.SetActive(true);
            ActiveHUD.SetParent(null);
        }

        public void UnloadHUD()
        {
            if(ActiveHUD != null)
            {
                ActiveHUD.SetParent(transform);
                ActiveHUD.gameObject.SetActive(false);
                ActiveHUD = null;
            }
        }
    }
}