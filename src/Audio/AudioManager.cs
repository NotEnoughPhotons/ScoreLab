using Il2CppSLZ.Marrow.Audio;
using UnityEngine;

using NEP.ScoreLab.Core;
using NEP.ScoreLab.Data;

namespace NEP.ScoreLab.Audio
{
    public static class AudioManager
    {
        public static void Initialize()
        {
            API.Value.OnValueAdded += OnValueReceived;
            API.Value.OnValueTierReached += OnValueReceived;
        }

        public static void Uninitialize()
        {
            API.Value.OnValueAdded -= OnValueReceived;
            API.Value.OnValueTierReached -= OnValueReceived;
        }

        private static void OnValueReceived(PackedValue value)
        {
            if (!Settings.UseAnnouncer)
            {
                return;
            }
            
            if(value.EventAudio != null)
            {
                Play(value.EventAudio.Clip, value.EventAudio.Volume, value.EventAudio.Pitch);
            }
        }

        public static void Play(AudioClip clip, float volume = 1f, float pitch = 1f)
        {
            BoneLib.Audio.PlayAtPoint(clip, Vector3.zero, BoneLib.Audio.InHead, volume, pitch, 0f);
        }
    }
}
