using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NEP.ScoreLab.UI
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class UIValueTween : MonoBehaviour
    {
        public UIValueTween(System.IntPtr ptr) : base(ptr) { }

        public int Value;
        public int TargetValue;
        public int Rate = 2;
        public Il2CppTMPro.TextMeshProUGUI text;

        private int _targetValue;
        private int _value;
        private int _rate;

        private void Awake()
        {
            text = GetComponent<Il2CppTMPro.TextMeshProUGUI>();
        }

        public void SetValue(int value)
        {
            _targetValue = value;
            TargetValue = _targetValue;
            _rate = (int)(Mathf.Abs(_targetValue - _value) / 0.5f);
        }

        public void Tween()
        {
            _value = (int)Mathf.MoveTowards(_value, _targetValue, _rate * Time.deltaTime);
            text.text = _value.ToString();
        }

        private void Update()
        {
            Tween();

            if(text == null)
            {
                return;
            }

            text.text = $"{Value}";
        }
    }
}

