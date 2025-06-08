using NEP.NEDebug.Console;
using NEP.ScoreLab.Data;
using UnityEngine;
using UnityEngine.UIElements;

namespace NEP.ScoreLab.Core
{
    public static class ScoreTracker
    {
        public static List<PackedValue> ActiveValues { get; private set; }

        public static int Score
        {
            get => _score;
        }
        public static int LastScore
        {
            get => _lastScore;
        }
        public static float Multiplier
        {
            get => _multiplier;
        }

        private static int _score = 0;
        private static int _lastScore = 0;
        private static float _multiplier = 1f;

        private static float _baseMultiplier = 1f;

        public static void Initialize()
        {
            ActiveValues = new List<PackedValue>();
        }

        public static void Update()
        {
            // Don't do anything until this list gets created
            if (ActiveValues == null)
            {
                return;
            }
            
            if (Input.GetKeyDown(KeyCode.RightBracket))
            {
                Add(ValueManager.Get(Data.EventType.Mult.Kill));
            }
            
            for (int i = 0; i < ActiveValues.Count; i++)
            {
                ActiveValues[i].OnUpdate();
            }
        }
        
        [NEConsoleCommand("scorelab.add")]
        public static void Add(string eventType)
        {
            Add(Create(eventType));
        }

        public static void Add(PackedValue value)
        {
            if (value == null)
            {
                return;
            }
            
            if (value.PackedValueType == PackedValue.PackedType.Score)
            {
                SetPackedScore((PackedScore)value);
            }
            else if (value.PackedValueType == PackedValue.PackedType.Multiplier)
            {
                SetPackedMultiplier((PackedMultiplier)value);
            }
        }

        public static void Remove(PackedValue value)
        {
            if (value.PackedValueType == PackedValue.PackedType.Score)
            {
                ActiveValues.Remove(value);

                value.OnValueRemoved();

                API.Value.OnValueRemoved?.Invoke(value);
            }
            else if (value.PackedValueType == PackedValue.PackedType.Multiplier)
            {
                ActiveValues.Remove(value);
                PackedMultiplier mult = value as PackedMultiplier;

                RemoveMultiplier(mult.AccumulatedMultiplier);
                
                value.OnValueRemoved();

                API.Value.OnValueRemoved?.Invoke(value);
            }
        }

        public static void AddScore(int score)
        {
            _lastScore = _score;
            _score += UnityEngine.Mathf.RoundToInt(score * _multiplier);
        }

        public static void AddMultiplier(float multiplier)
        {
            _multiplier += multiplier;
        }

        public static void SetMultiplier(float multiplier)
        {
            _multiplier = multiplier;
        }

        [NEConsoleCommand("scorelab.reset_score")]
        public static void ResetScore()
        {
            _score = 0;
            _lastScore = 0;
        }

        [NEConsoleCommand("scorelab.reset_multiplier")]
        public static void ResetMultiplier()
        {
            _multiplier = 1f;
        }

        public static void ResetHighScore()
        {

        }

        public static void RemoveMultiplier(float multiplier)
        {
            if (_multiplier < _baseMultiplier)
            {
                _multiplier = _baseMultiplier;
            }

            _multiplier -= multiplier;
        }

        public static bool CheckDuplicate(PackedValue value)
        {
            foreach (var val in ActiveValues)
            {
                if (val.eventType == value.eventType)
                {
                    return true;
                }
            }

            return false;
        }

        private static void SetPackedScore(PackedScore score)
        {
            if (score == null)
            {
                return;
            }

            if (!CheckDuplicate(score))
            {
                InitializeValue(score);
                AddScore(score.Score);
                ActiveValues.Add(score);

                API.Value.OnValueAdded?.Invoke(score);
                return;
            }

            if (score.Tiers != null)
            {
                var parent = GetClone<PackedScore>(score);
                var currentTier = (PackedScore)parent.CurrentTier;

                parent.ToNextTier();
                parent.TierRequirement = currentTier.TierRequirement;
                parent.SetDecayTime(currentTier.DecayTime);

                AddScore(currentTier.Score);
            }
            else if (score.Stackable)
            {
                var _scoreInList = GetClone<PackedScore>(score);

                AddScore(score.Score);

                _scoreInList.SetDecayTime(_scoreInList.DecayTime);
                _scoreInList.AccumulatedScore += _scoreInList.Score;

                API.Value.OnValueAccumulated?.Invoke(_scoreInList);
            }
            else
            {
                PackedScore copy = CopyFromScore(score);
                InitializeValue(copy);
                AddScore(copy.Score);
                ActiveValues.Add(copy);

                API.Value.OnValueAdded?.Invoke(copy);
            }
        }

        private static void SetPackedMultiplier(PackedMultiplier multiplier)
        {
            if (multiplier == null)
            {
                return;
            }

            if (!CheckDuplicate(multiplier))
            {
                InitializeValue(multiplier);
                AddMultiplier(multiplier.Multiplier);
                ActiveValues.Add(multiplier);

                API.Value.OnValueAdded?.Invoke(multiplier);
                return;
            }

            if (multiplier.Tiers != null)
            {
                var parent = GetClone<PackedMultiplier>(multiplier);
                var currentTier = (PackedMultiplier)parent.CurrentTier;

                parent.ToNextTier();
                parent.TierRequirement = currentTier.TierRequirement;
                parent.SetDecayTime(currentTier.DecayTime);

                AddMultiplier(currentTier.Multiplier);
                multiplier.AccumulatedMultiplier += currentTier.Multiplier;
                API.Value.OnValueTierReached?.Invoke(currentTier);
            }
            else if (multiplier.Stackable)
            {
                var _multInList = GetClone<PackedMultiplier>(multiplier);

                AddMultiplier(multiplier.Multiplier);

                _multInList.SetDecayTime(_multInList.DecayTime);
                _multInList.AccumulatedMultiplier += _multInList.Multiplier;

                API.Value.OnValueAccumulated?.Invoke(_multInList);
            }
            else
            {
                PackedMultiplier copy = CopyFromMult(multiplier);
                InitializeValue(copy);
                AddMultiplier(multiplier.Multiplier);
                ActiveValues.Add(copy);

                API.Value.OnValueAdded?.Invoke(copy);
            }
        }

        private static PackedScore CopyFromScore(PackedScore original)
        {
            PackedScore score = new PackedScore()
            {
                eventType = original.eventType,
                Name = original.Name,
                Score = original.Score,
                EventAudio = original.EventAudio,
                DecayTime = original.DecayTime
            };

            return score;
        }

        private static PackedMultiplier CopyFromMult(PackedMultiplier original)
        {
            PackedMultiplier score = new PackedMultiplier()
            {
                eventType = original.eventType,
                Name = original.Name,
                Multiplier = original.Multiplier,
                EventAudio = original.EventAudio,
                DecayTime = original.DecayTime,
                Condition = original.Condition
            };

            return score;
        }

        private static PackedValue Create(string eventType)
        {
            var Event = ValueManager.Get(eventType);

            if (Event == null)
            {
                return null;
            }
            
            if (Event.PackedValueType == PackedValue.PackedType.Score)
            {
                var scoreEvent = (PackedScore)Event;

                var score = new PackedScore()
                {
                    eventType = scoreEvent.eventType,
                    Stackable = scoreEvent.Stackable,
                    DecayTime = scoreEvent.DecayTime,
                    Name = scoreEvent.Name,
                    Score = scoreEvent.Score,
                    EventAudio = scoreEvent.EventAudio,
                    TierRequirement = scoreEvent.TierRequirement,
                    Tiers = scoreEvent.Tiers,
                };

                return score;
            }
            else if (Event.PackedValueType == PackedValue.PackedType.Multiplier)
            {
                var multEvent = (PackedMultiplier)Event;

                var mult = new PackedMultiplier()
                {
                    eventType = multEvent.eventType,
                    Stackable = multEvent.Stackable,
                    DecayTime = multEvent.DecayTime,
                    Name = multEvent.Name,
                    Multiplier = multEvent.Multiplier,
                    Condition = multEvent.Condition,
                    EventAudio = multEvent.EventAudio,
                    TierRequirement = multEvent.TierRequirement,
                    Tiers = multEvent.Tiers
                };

                return mult;
            }

            return null;
        }

        private static void InitializeValue(PackedValue value)
        {
            value.OnValueCreated();
            value.SetDecayTime(value.DecayTime);
        }

        private static T GetClone<T>(PackedValue value) where T : PackedValue
        {
            return (T)ActiveValues.Find((match) => match.eventType == value.eventType);
        }
    }
}

