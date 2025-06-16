using Il2CppInterop.Runtime.Attributes;
using Il2CppInterop.Runtime.InteropTypes.Fields;
using Il2CppUltEvents;
using MelonLoader;
using NEP.ScoreLab.Core;
using UnityEngine;

namespace NEP.ScoreLab.HUD
{
    [RegisterTypeInIl2Cpp]
    public class HUDEvents : MonoBehaviour
    {
        public HUDEvents(System.IntPtr ptr) : base(ptr) { }

        public Il2CppReferenceField<UltEventHolder> OnScoreUpdateEvent;
        public Il2CppReferenceField<UltEventHolder> OnMultiplierUpdateEvent;

        private void Awake()
        {
            API.Value.OnValueAdded += (value) => OnScoreUpdateEvent_Callback();
        }

        private void OnDestroy()
        {
            API.Value.OnValueAdded -= (value) => OnScoreUpdateEvent_Callback();
        }

        [HideFromIl2Cpp]
        private void OnScoreUpdateEvent_Callback()
        {
            OnScoreUpdateEvent.Get()?.Invoke();
        }

        [HideFromIl2Cpp]
        private void OnMultiplierUpdateEvent_Callback()
        {
            OnMultiplierUpdateEvent.Get()?.Invoke();
        }
    }
}