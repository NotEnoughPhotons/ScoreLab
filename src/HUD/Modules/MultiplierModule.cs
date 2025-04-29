using NEP.ScoreLab.Core;
using NEP.ScoreLab.Data;

using UnityEngine;

namespace NEP.ScoreLab.HUD
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class MultiplierModule : Module
    {
        public MultiplierModule(System.IntPtr ptr) : base(ptr) { }

        private PackedMultiplier _packedMultiplier { get => (PackedMultiplier)_packedValue; }

        private float _currentValue;
        private float _targetValue;
        private float _rate;
        
        private void Awake()
        {
            if (name == "MultiplierDescriptor")
            {
                ModuleType = UIModuleType.Descriptor;
            }
        }

        public override void OnModuleEnable()
        {
            base.OnModuleEnable();

            if (_packedValue == null)
            {
                return;
            }

            if (_packedMultiplier == null)
            {
                return;
            }

            if (ModuleType == UIModuleType.Main)
            {
                SetText(_value, $"{ScoreTracker.Multiplier.ToString()}x");
            }
            else if (ModuleType == UIModuleType.Descriptor)
            {
                if (PackedValue.Stackable)
                {
                    if (PackedValue.TierEventType != null)
                    {
                        SetText(_title, _packedMultiplier.Name);
                        SetText(_value, $"{_packedMultiplier.Multiplier.ToString()}x");
                    }
                    else
                    {
                        SetText(_title, _packedMultiplier.Name);
                        SetText(_value, $"{_packedMultiplier.AccumulatedMultiplier.ToString()}x");
                    }
                }
                else
                {
                    SetText(_title, _packedMultiplier.Name);
                    SetText(_value, $"{_packedMultiplier.Multiplier.ToString()}x");
                }
            }

            if (_timeBar != null)
            {
                if (_packedMultiplier.Condition != null)
                {
                    _timeBar.gameObject.SetActive(false);
                }
                else
                {
                    _timeBar.gameObject.SetActive(true);
                    SetMaxValueToBar(_timeBar, _packedMultiplier.DecayTime);
                    SetBarValue(_timeBar, _packedMultiplier.DecayTime);
                }
            }
        }

        public override void OnModuleDisable()
        {

        }

        public override void OnUpdate()
        {
            if (ModuleType == UIModuleType.Main)
            { 
                SetTweenValue(ScoreTracker.Multiplier);
                _currentValue = Mathf.MoveTowards(_currentValue, _targetValue, _rate * Time.unscaledDeltaTime);
                if (Mathf.Approximately(_currentValue, _targetValue))
                {
                    _currentValue = _targetValue;
                }
                
                SetText(_value, _currentValue.ToString("F2") + "x");
            }
            
            if (_packedMultiplier != null)
            {
                if (_packedMultiplier.condition != null)
                {
                    if (!_packedMultiplier.condition())
                    {
                        UpdateDecay();
                    }
                }
                else
                {
                    UpdateDecay();
                }
            }
            else
            {
                UpdateDecay();
            }

            if (_timeBar != null)
            {
                SetBarValue(_timeBar, _tDecay);
            }
        }
        
        private void SetTweenValue(float value)
        {
            _targetValue = value;
            _rate = Mathf.Abs(_targetValue - _currentValue) / 1.0f;
        }
    }
}
