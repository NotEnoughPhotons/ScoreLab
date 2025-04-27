using Il2CppSystem.Reflection;
using UnityEngine;

using NEP.ScoreLab.Core;
using NEP.ScoreLab.Data;

namespace NEP.ScoreLab.HUD
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class ScoreModule : Module
    {
        public ScoreModule(System.IntPtr ptr) : base(ptr) { }

        private PackedScore _packedScore { get => (PackedScore)_packedValue; }

        private float _targetValue;
        private float _currentValue;
        private float _rate;
        
        private void Awake()
        {
            if (name == "ScoreDescriptor")
            {
                ModuleType = UIModuleType.Descriptor;
            }
        }

        public override void OnModuleEnable()
        {
            base.OnModuleEnable();

            if (ModuleType == UIModuleType.Main)
            {
                SetText(_value, ScoreTracker.Instance.Score);
            }
            
            if (_packedValue == null)
            {
                return;
            }
            
            if (ModuleType == UIModuleType.Descriptor)
            {
                if (PackedValue.Stackable)
                {
                    if(PackedValue.TierEventType != null)
                    {
                        SetText(_title, _packedScore.Name);
                        SetText(_value, _packedScore.Score);
                    }
                    else
                    {
                        SetText(_title, _packedScore.Name);
                        SetText(_value, _packedScore.AccumulatedScore);
                    }
                }
                else
                {
                    SetText(_title, _packedScore.Name);
                    SetText(_value, _packedScore.Score);
                }
            }
        }

        public override void OnModuleDisable()
        {

        }

        public override void OnUpdate()
        {
            UpdateDecay();

            if (ModuleType == UIModuleType.Main)
            { 
                SetTweenValue(ScoreTracker.Instance.Score);
                _currentValue = Mathf.MoveTowards(_currentValue, _targetValue, _rate * Time.unscaledDeltaTime);
                if (Mathf.Approximately(_currentValue, _targetValue))
                {
                    _currentValue = _targetValue;
                }
                
                SetText(_value, _currentValue.ToString("N0"));
            }
        }

        private void SetTweenValue(int value)
        {
            _targetValue = value;
            _rate = Mathf.Abs(_targetValue - _currentValue) / 1.0f;
        }
    }
}