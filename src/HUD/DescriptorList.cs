using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NEP.ScoreLab.Core;
using NEP.ScoreLab.Data;

namespace NEP.ScoreLab.HUD
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class DescriptorList : MonoBehaviour
    {
        public DescriptorList(System.IntPtr ptr) : base(ptr) { }

        public List<Module> ActiveModules;

        public PackedValue.PackedType packedType { get; set; }
        public GameObject ModulePrefab { get; set; }
        public int count = 6;

        public List<Module> Modules;
        
        private void Awake()
        {
            Modules = new List<Module>();
            ActiveModules = new List<Module>();

            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                Modules.Add(child.GetComponent<Module>());
            }

            // TODO: Come up with a better system for this
            if (name == "ScoreDescriptors")
            {
                packedType = PackedValue.PackedType.Score;
            }
            else if (name == "MultiplierDescriptors")
            {
                packedType = PackedValue.PackedType.Multiplier;
            }
        }

        private void OnEnable()
        {
            API.UI.OnModulePostDecayed += (item) => ActiveModules.Remove(item);

            API.Value.OnValueAdded += SetModuleActive;
            API.Value.OnValueAccumulated += SetModuleActive;
            API.Value.OnValueTierReached += SetModuleActive;
        }

        private void OnDisable()
        {
            API.UI.OnModulePostDecayed -= (item) => ActiveModules.Remove(item);

            API.Value.OnValueAdded -= SetModuleActive;
            API.Value.OnValueAccumulated -= SetModuleActive;
            API.Value.OnValueTierReached -= SetModuleActive;
        }

        public void SetPackedType(int packedType)
        {
            this.packedType = (PackedValue.PackedType)packedType;
        }

        public void SetModuleActive(PackedValue value)
        {
            if (Modules == null || Modules.Count == 0)
            {
                return;
            }

            if (value.PackedValueType != packedType)
            {
                return;
            }

            foreach (var module in Modules)
            {
                if (!module.gameObject.activeInHierarchy)
                {
                    module.AssignPackedData(value);
                    module.SetDecayTime(value.DecayTime);
                    module.SetPostDecayTime(0.5f);

                    module.gameObject.SetActive(true);

                    ActiveModules.Add(module);
                    break;
                }
                
                if (ActiveModules.Contains(module) && !module.IsDecaying())
                {
                    if (module.PackedValue.eventType == value.eventType)
                    {
                        if (value.Stackable)
                        {
                            module.AssignPackedData(value);
                            module.OnModuleEnable();
                            module.SetDecayTime(value.DecayTime);
                            module.SetPostDecayTime(0.5f);
                            break;
                        }

                        if (value.Tiers != null)
                        {
                            module.AssignPackedData(value);
                            module.OnModuleEnable();
                            module.SetDecayTime(value.DecayTime);
                            module.SetPostDecayTime(0.5f);
                            break;
                        }
                    }
                }
            }
        }

        public void PoolObjects()
        {
            if (ModulePrefab == null)
            {
                return;
            }

            for (int i = 0; i < count; i++)
            {
                var go = GameObject.Instantiate(ModulePrefab, transform);
                var module = go.GetComponent<Module>();
                module.ModuleType = Module.UIModuleType.Descriptor;
                Modules.Add(module);
            }
        }
    }
}

