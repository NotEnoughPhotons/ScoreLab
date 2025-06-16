using MelonLoader;

using UnityEngine;

using Il2CppTMPro;

namespace NEP.ScoreLab.HUD
{
    [RegisterTypeInIl2Cpp]
    public class HUDText : MonoBehaviour
    {
        public HUDText(System.IntPtr ptr) : base(ptr) { }
        
        public bool TextMeshExists => m_textMesh != null;
        public bool TextMeshUGUIExists => m_textMeshUGUI != null;
        
        protected TextMeshPro m_textMesh;
        protected TextMeshProUGUI m_textMeshUGUI;

        protected virtual void Awake()
        {
            m_textMesh = GetComponent<TextMeshPro>();
            m_textMeshUGUI = GetComponent<TextMeshProUGUI>();
        }
        
        public virtual void Set(string text)
        {
            if (TextMeshExists)
            {
                m_textMesh.text = text;
            }

            if (TextMeshUGUIExists)
            {
                m_textMeshUGUI.text = text;
            }
        }
    }
}