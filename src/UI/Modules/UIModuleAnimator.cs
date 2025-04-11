using UnityEngine;

using NEP.ScoreLab.Core;

namespace NEP.ScoreLab.UI
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class UIModuleAnimator : MonoBehaviour
    {
        public UIModuleAnimator(System.IntPtr ptr) : base(ptr) { }

        public Animator Animator;

        private UIModule _module;

        private void Awake()
        {
            _module = GetComponent<UIModule>();
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

        private void OnModuleEnabled(UIModule module)
        {
            if (_module != module)
            {
                return;
            }

            if (_module.ModuleType == UIModule.UIModuleType.Descriptor)
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

        private void OnModuleDecayed(UIModule module)
        {
            if (_module != module)
            {
                return;
            }

            if (_module.ModuleType == UIModule.UIModuleType.Descriptor)
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
