using System;
using UnityEngine;

using NEP.ScoreLab.Core;

namespace NEP.ScoreLab.Data
{
    [Serializable]
    public class PackedMultiplier : PackedValue
    {
        public PackedMultiplier()
        {
            
        }

        public PackedMultiplier(JSONMult mult, bool useTiers = false)
        {
            eventType = mult.EventType;
            DecayTime = mult.DecayTime;
            Stackable = mult.Stackable;
            Name = mult.Name;
            Multiplier = mult.Multiplier;
            Condition = mult.Condition;
            TierRequirement = mult.TierRequirement;
            EventAudio = new PackedAudioParams(mult.EventAudio);
                
            if (useTiers)
            {
                TierEventType = mult.TierEventType;
            }
        }
        
        public override PackedType PackedValueType => PackedType.Multiplier;
        public float Multiplier;
        public float AccumulatedMultiplier;
        public float Elapsed { get => _tDecay; }
        public string Condition;
        public Func<bool> condition { get; private set; }

        private bool _timed;
        private bool _timeBegin;

        public override void OnValueCreated()
        {
            if (DecayTime != 0f)
            {
                _timed = true;
            }

            condition = API.GameConditions.GetCondition(Condition);
            AccumulatedMultiplier = Multiplier;
            _tDecay = DecayTime;
        }

        public override void OnValueRemoved()
        {
            _timeBegin = false;
            ResetTier();
        }

        public override void OnUpdate()
        {
            if (condition != null)
            {
                if (!condition())
                {
                    ScoreTracker.Remove(this);
                }
            }

            OnUpdateDecay();
        }

        public override void OnUpdateDecay()
        {
            if (_timed)
            {
                if (!_timeBegin)
                {
                    API.Multiplier.OnMultiplierTimeBegin?.Invoke(this);
                    _timeBegin = true;
                }

                if (_tDecay <= 0f)
                {
                    API.Multiplier.OnMultiplierTimeExpired?.Invoke(this);
                    _tDecay = DecayTime;
                    ScoreTracker.Remove(this);
                }

                _tDecay -= Time.deltaTime;
            }
        }
    }
}