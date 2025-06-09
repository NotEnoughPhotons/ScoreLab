using UnityEngine;

namespace NEP.ScoreLab.Data
{
    public class PackedAudioParams
    {
        public PackedAudioParams()
        {
            Clips = null;
            Volume = 1f;
            Pitch = 1f;
        }

        public PackedAudioParams(JSONAudioParams audioParams)
        {
            Clips = new AudioClip[audioParams.sounds.Length];

            for (int i = 0; i < Clips.Length; i++)
            {
                Clips[i] = HUDLoader.HUDAudioBank[audioParams.sounds[i]];
            }
            
            Volume = audioParams.volume;
            Pitch = audioParams.pitch;
        }
        
        public AudioClip[] Clips { get; private set; }
        public float Volume { get; private set; }
        public float Pitch { get; private set; }
    }
}