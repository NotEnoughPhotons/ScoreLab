using UnityEngine;

using BoneLib;

using Il2CppSLZ.Bonelab;
using Il2CppSLZ.Marrow;
using Il2CppSLZ.Marrow.PuppetMasta;
using Il2CppSLZ.Marrow.AI;
using Il2CppSLZ.Marrow.Interaction;

using NEP.ScoreLab.Data;

using Avatar = Il2CppSLZ.VRMK.Avatar;
using EventType = NEP.ScoreLab.Data.EventType;

namespace NEP.ScoreLab.Core
{
    public static class ScoreDirector
    {
        public static class Patches
        {
            [HarmonyLib.HarmonyPatch(typeof(Projectile), nameof(Projectile.Awake))]
            public static class ProjectilePatch
            {
                public static void Postfix(Projectile __instance)
                {
                    Action<Collider, Vector3, Vector3> action = OnProjectileCollision;
                    __instance.onCollision.AddListener(action);
                }

                private static void OnProjectileCollision(Collider collider, Vector3 world, Vector3 normal)
                {
                    MarrowBody head = MarrowBody.Cache.Get(collider.gameObject);

                    if (head == null)
                    {
                        return;
                    }
                    
                    TriggerRefProxy proxy = head.GetComponent<TriggerRefProxy>();

                    if (proxy == null)
                    {
                        return;
                    }

                    if (proxy.aiManager.isDead)
                    {
                        return;
                    }
                    
                    if (proxy.targetHead.gameObject == head.gameObject)
                    {
                        ScoreTracker.Add(EventType.Score.Headshot);
                    }
                }
            }
            
            [HarmonyLib.HarmonyPatch(typeof(RigManager), nameof(RigManager.SwitchAvatar))]
            public static class RigManagerSwapAvatarPatch
            {
                public static void Postfix(Avatar newAvatar)
                {
                    _playerRecentlySwappedAvatars = true;
                }
            }
            
            [HarmonyLib.HarmonyPatch(typeof(BehaviourCrablet))]
            [HarmonyLib.HarmonyPatch(nameof(BehaviourCrablet.AttachToFace))]
            public static class CrabletAttachToFacePatch
            {
                public static void Postfix(Rigidbody face, TriggerRefProxy trp, bool preAttach = false, bool isPlayer = true)
                {
                    if (isPlayer)
                    {
                        return;
                    }

                    if (trp.npcType == TriggerRefProxy.NpcType.Crablet)
                    {
                        ScoreTracker.Add(EventType.Score.Crabcest);
                    }
                    else
                    {
                        ScoreTracker.Add(EventType.Score.Facehug);
                    }
                }
            }
            
            [HarmonyLib.HarmonyPatch(typeof(Seat))]
            [HarmonyLib.HarmonyPatch(nameof(Seat.Register))]
            public static class RegisterSeatPatch
            {
                public static void Postfix(RigManager rM)
                {
                    IsPlayerSeated = true;
                    ScoreTracker.Add(EventType.Mult.Seated);
                }
            }

            [HarmonyLib.HarmonyPatch(typeof(Seat))]
            [HarmonyLib.HarmonyPatch(nameof(Seat.DeRegister))]
            public static class DeRegisterSeatPatch
            {
                public static void Postfix()
                {
                    IsPlayerSeated = false;
                }
            }

            [HarmonyLib.HarmonyPatch(typeof(Player_Health))]
            [HarmonyLib.HarmonyPatch(nameof(Player_Health.LifeSavingDamgeDealt))]
            public static class SecondWindPatch
            {
                public static void Postfix()
                {
                    ScoreTracker.Add(EventType.Mult.SecondWind);
                }
            }

            [HarmonyLib.HarmonyPatch(typeof(Arena_GameController))]
            [HarmonyLib.HarmonyPatch(nameof(Arena_GameController.Awake))]
            public static class ArenaGameControllerAwakePatch
            {
                public static void Postfix(Arena_GameController __instance)
                {
                    __instance.onRoundEnd.AddListener(new System.Action(() =>
                    {
                        ScoreTracker.Add(EventType.Score.GameRoundCompleted);
                    }));
                    
                    __instance.onWaveEnd.AddListener(new System.Action(() =>
                    {
                        ScoreTracker.Add(EventType.Score.GameWaveCompleted);
                    }));
                }
            }

            [HarmonyLib.HarmonyPatch(typeof(PhysicsRig))]
            [HarmonyLib.HarmonyPatch(nameof(PhysicsRig.OnUpdate))]
            public static class PhysRigPatch
            {
                private static bool _midAirTargetBool;
                private static float _tMidAirDelay = 0.5f;
                private static float _tAirTime;

                private static bool _ragdolledTargetBool;
                private static float _tRagdollDelay = 0.5f;
                private static float _tRagdollTime;

                public static void Postfix(PhysicsRig __instance)
                {
                    IsPlayerInAir = !__instance.physG.isGrounded;

                    if (IsPlayerInAir)
                    {
                        if (!_midAirTargetBool)
                        {
                            _tAirTime += Time.deltaTime;

                            if(_tAirTime > _tMidAirDelay)
                            {
                                ScoreTracker.Add(EventType.Mult.MidAir);
                                _midAirTargetBool = true;
                            }
                        }
                    }
                    else
                    {
                        _tAirTime = 0f;
                        _midAirTargetBool = false;
                    }

                    if (IsPlayerRagdolled)
                    {
                        if (!_ragdolledTargetBool)
                        {
                            // ScoreTracker.Instance.Add(Data.EventType.Mult.Ragolled);
                            _ragdolledTargetBool = true;
                        }
                    }
                    else
                    {
                        _ragdolledTargetBool = false;
                    }

                    if (_playerRecentlySwappedAvatars)
                    {
                        ScoreTracker.Add(EventType.Mult.SwappedAvatars);
                        _playerRecentlySwappedAvatars = false;
                    }
                }
            }

            [HarmonyLib.HarmonyPatch(typeof(PhysicsRig))]
            [HarmonyLib.HarmonyPatch(nameof(PhysicsRig.RagdollRig))]
            public static class PhysRigRagdollPatch
            {
                public static void Postfix(PhysicsRig __instance)
                {
                    IsPlayerRagdolled = true;
                }
            }

            [HarmonyLib.HarmonyPatch(typeof(PhysicsRig))]
            [HarmonyLib.HarmonyPatch(nameof(PhysicsRig.UnRagdollRig))]
            public static class PhysRigUnRagdollPatch
            {
                public static void Postfix(PhysicsRig __instance)
                {
                    IsPlayerRagdolled = false;
                }
            }

            [HarmonyLib.HarmonyPatch(typeof(TimeManager), nameof(TimeManager.OnPostTimeUpdate))]
            public static class TimeManagerPatch
            {
                public static void Postfix()
                {
                    if (TimeManager.slowMoEnabled)
                    {
                        ScoreTracker.Add(EventType.Mult.SlowMo);
                    }
                }
            }

            public static void InitPatches()
            {
                Hooking.OnNPCKillStart += OnAIDeath;
            }

            public static void OnAIDeath(BehaviourBaseNav behaviour)
            {
                if(!behaviour.sensors.isGrounded)
                {
                    ScoreTracker.Add(EventType.Score.EnemyMidAirKill);
                }

                ScoreTracker.Add(ValueManager.Get(EventType.Score.Kill));
                ScoreTracker.Add(ValueManager.Get(EventType.Mult.Kill));
            }
        }
        
        public static bool IsPlayerMoving = false;
        public static bool IsPlayerInAir = false;
        public static bool IsPlayerSeated = false;
        public static bool IsPlayerRagdolled = false;
        public static bool IsSlowMoEnabled = false;

        private static bool _playerRecentlySwappedAvatars = false;
    }
}
