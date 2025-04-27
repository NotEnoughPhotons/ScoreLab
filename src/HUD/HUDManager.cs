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

            for(int i = 0; i < DataManager.UI.LoadedUIObjects.Count; i++)
            {
                var _object = GameObject.Instantiate(DataManager.UI.LoadedUIObjects[i]);
                _object.name = _object.name.Remove(_object.name.Length - 7);

                var controller = _object.GetComponent<HUD>();

                // Not a valid UI
                if(controller == null)
                {
                    continue;
                }

                LoadedHUDs.Add(controller);
                controller.SetParent(transform);
                controller.gameObject.SetActive(false);
            }
        }

        private void Start()
        {
            LoadHUD("Tabloid");
        }

        public void LoadHUD(string name)
        {
            UnloadHUD();
            foreach (var _controller in LoadedHUDs)
            {
                if (DataManager.UI.GetHUDName(_controller.gameObject) == name)
                {
                    ActiveHUD = _controller;
                    break;
                }
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