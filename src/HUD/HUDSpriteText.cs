using System.Text;
using UnityEngine;

using Il2CppTMPro;

namespace NEP.ScoreLab.HUD
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class HUDSpriteText : HUDText
    {
        public HUDSpriteText(System.IntPtr ptr) : base(ptr) { }

        private StringBuilder m_stringBuilder;

        protected override void Awake()
        {
            base.Awake();
            
            m_stringBuilder = new StringBuilder();
        }

        private bool HasSpriteAsset()
        {
            if (TextMeshExists)
            {
                return m_textMesh.spriteAsset != null;
            }
            
            if (TextMeshUGUIExists)
            {
                return m_textMeshUGUI.spriteAsset != null;
            }

            return false;
        }

        public override void Set(string text)
        {
            if (!HasSpriteAsset())
            {
                return;
            }

            if (m_stringBuilder == null)
            {
                throw new NullReferenceException("Missing string builder in: " + gameObject.name);
            }

            for (int i = 0; i < text.Length; i++)
            {
                if (!char.IsDigit(text[i]))
                {
                    continue;
                }
                
                int spriteIndex = int.Parse(text[i].ToString());
                m_stringBuilder.Append("<sprite index=");
                m_stringBuilder.Append(spriteIndex);
                m_stringBuilder.Append('>');
            }
            
            base.Set(m_stringBuilder.ToString());
            m_stringBuilder.Clear();
        }
    }
}