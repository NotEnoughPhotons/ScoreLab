using Il2CppInterop.Runtime.Attributes;
using UnityEngine;

using NEP.ScoreLab.Core;

namespace NEP.ScoreLab.HUD
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class ModuleAnimator : MonoBehaviour
    {
        public ModuleAnimator(System.IntPtr ptr) : base(ptr) { }

        public Animator Animator;

        private Module _module;

        private void Awake()
        {
            _module = GetComponent<Module>();
            Animator = GetComponent<Animator>();
        }

        private void OnEnable()
        {
            //API.UI.OnModuleEnabled += OnModuleEnabled;
            API.Value.OnValueTierReached += (data) => OnTierReached();
            API.UI.OnModuleDecayed += OnModuleDecayed;
        }

        private void OnDisable()
        {
            //API.UI.OnModuleEnabled -= OnModuleEnabled;
            API.Value.OnValueTierReached -= (data) => OnTierReached();
            API.UI.OnModuleDecayed -= OnModuleDecayed;
        }

        private void PlayAnimation(string name)
        {
            if (Animator == null)
            {
                return;
            }
            
            Animator.Play(name, -1, 0f);
        }

        [HideFromIl2Cpp]
        private void OnModuleEnabled(Module module)
        {
            if (_module != module)
            {
                return;
            }

            if (_module.ModuleType == Module.UIModuleType.Descriptor)
            {
                PlayAnimation("descriptor_show");
            }
            else
            {
                PlayAnimation("show");
            }
        }

        private void OnTierReached()
        {
            PlayAnimation("tier_reached");
        }

        [HideFromIl2Cpp]
        private void OnModuleDecayed(Module module)
        {
            if (_module != module)
            {
                return;
            }

            if (_module.ModuleType == Module.UIModuleType.Descriptor)
            {
                PlayAnimation("descriptor_hide");
            }
            else
            {
                PlayAnimation("hide");
            }
        }
    }
}
