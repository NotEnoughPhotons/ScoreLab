using System;

using NEP.ScoreLab.Core;

namespace NEP.ScoreLab.Data
{
    [Serializable]
    public class PackedScore : PackedValue
    {
        public PackedScore()
        {
            
        }
        
        public PackedScore(JSONScore score, bool useTiers = false)
        {
            eventType = score.EventType;
            DecayTime = score.DecayTime;
            Stackable = score.Stackable;
            Name = score.Name;
            Score = score.Score;
            TierRequirement = score.TierRequirement;
            EventAudio = new PackedAudioParams(score.EventAudio);
            
            if (useTiers)
            {
                TierEventType = score.TierEventType;
            }
            else
            {
                AccumulatedScore = score.Score;
            }
        }
        
        public override PackedType PackedValueType => PackedType.Score;
        public int Score;
        public int AccumulatedScore;

        public override void OnValueCreated()
        {
            _tDecay = DecayTime;
            AccumulatedScore = Score;
        }

        public override void OnValueRemoved()
        {
            ResetTier();
        }

        public override void OnUpdate()
        {
            OnUpdateDecay();
        }

        public override void OnUpdateDecay()
        {
            if (_tDecay <= 0f)
            {
                ScoreTracker.Remove(this);
            }

            _tDecay -= UnityEngine.Time.deltaTime;
        }
    }
}

