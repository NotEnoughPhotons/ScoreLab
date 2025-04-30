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
                Play(value.EventAudio.Clip);
            }
        }

        public static void Play(AudioClip clip)
        {
            BoneLib.Audio.Play2DOneShot(clip, BoneLib.Audio.InHead);
        }
    }
}
