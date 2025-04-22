using UnityEngine;

using NEP.ScoreLab.Core;
using NEP.ScoreLab.Data;

namespace NEP.ScoreLab.UI
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class UIController : MonoBehaviour
    {
        public UIController(System.IntPtr ptr) : base(ptr) { }

        public UIModule ScoreModule { get; set; }
        public UIModule MultiplierModule { get; set; }
        public UIModule HighScoreModule { get; set; }

        public Transform followTarget;

        private void Awake()
        {
            if(transform.Find("Main_Score") != null)
            {
                ScoreModule = transform.Find("Main_Score").GetComponent<UIScoreModule>();
            }

            if (transform.Find("Main_Multiplier"))
            {
                MultiplierModule = transform.Find("Main_Multiplier").GetComponent<UIMultiplierModule>();
            }
        }

        private void OnEnable()
        {
            UpdateModule(null);
            UpdateModule(null);

            API.Value.OnValueAdded += UpdateModule;
            API.Value.OnValueTierReached += UpdateModule;
            API.Value.OnValueAccumulated += UpdateModule;
            API.Value.OnValueRemoved += UpdateModule;
        }

        private void OnDisable()
        {
            API.Value.OnValueAdded -= UpdateModule;
            API.Value.OnValueTierReached -= UpdateModule;
            API.Value.OnValueAccumulated -= UpdateModule;
            API.Value.OnValueRemoved -= UpdateModule;
        }

        private void Start()
        {
            if(BoneLib.Player.GetPhysicsRig() != null)
            {
                followTarget = BoneLib.Player.ControllerRig.m_head;
            }
        }
        
        // For being attached to a physical point on the body
        private void FixedUpdate()
        {
            if (followTarget == null)
            {
                return;
            }

            Vector3 move = Vector3.Lerp(transform.position, followTarget.position + followTarget.forward * Settings.DistanceToCamera, Settings.MovementSmoothness * Time.deltaTime);
            Quaternion lookRot = Quaternion.LookRotation(followTarget.forward);

            transform.position = move;
            transform.rotation = lookRot;

            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i) != null)
                {
                    //transform.GetChild(i).LookAt(followTarget);
                }
            }
        }

        public void SetParent(Transform parent)
        {
            transform.SetParent(parent);
        }

        public void SetScoreModule(UIModule module)
        {
            this.ScoreModule = module;
        }

        public void SetMultiplierModule(UIModule module)
        {
            this.MultiplierModule = module;
        }

        public void UpdateModule(PackedValue data)
        {
            if (data is PackedScore)
            {
                ScoreModule.AssignPackedData(data);
                ScoreModule.OnModuleEnable();
            }
            else if (data is PackedMultiplier)
            {
                MultiplierModule.AssignPackedData(data);
                MultiplierModule.OnModuleEnable();
            }
        }
    }
}
