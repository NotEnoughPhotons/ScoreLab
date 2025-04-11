using UnityEngine;

using NEP.ScoreLab.Data;

using BoneLib;

using Il2CppSLZ.Bonelab;
using Il2CppSLZ.Marrow;
using Il2CppSLZ.Marrow.PuppetMasta;
using Il2CppSLZ.Marrow.AI;
using Avatar = Il2CppSLZ.VRMK.Avatar;

namespace NEP.ScoreLab.Core
{
    public static class ScoreDirector
    {
        public static class Patches
        {
            [HarmonyLib.HarmonyPatch(typeof(RigManager))]
            [HarmonyLib.HarmonyPatch(nameof(RigManager.SwapAvatar))]
            public static class RigManagerSwapAvatarPatch
            {
                public static void Postfix(RigManager __instance, Avatar avatar)
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
                        ScoreTracker.Instance.Add(Data.EventType.Score.Crabcest);
                    }
                    else
                    {
                        ScoreTracker.Instance.Add(Data.EventType.Score.Facehug);
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
                    ScoreTracker.Instance.Add(Data.EventType.Mult.Seated);
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
                    ScoreTracker.Instance.Add(Data.EventType.Mult.SecondWind);
                }
            }

            [HarmonyLib.HarmonyPatch(typeof(Arena_Stats))]
            [HarmonyLib.HarmonyPatch(nameof(Arena_Stats.RoundAchieved))]
            public static class EndOfRoundPatch
            {
                public static void Postfix()
                {
                    ScoreTracker.Instance.Add(Data.EventType.Score.GameRoundCompleted);
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
                            _tAirTime += UnityEngine.Time.deltaTime;

                            if(_tAirTime > _tMidAirDelay)
                            {
                                ScoreTracker.Instance.Add(Data.EventType.Mult.MidAir);
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
                        ScoreTracker.Instance.Add(Data.EventType.Mult.SwappedAvatars);
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

            public static void InitPatches()
            {
                Hooking.OnNPCKillStart += OnAIDeath;
            }

            public static void OnAIDeath(BehaviourBaseNav behaviour)
            {
                if(!behaviour.sensors.isGrounded)
                {
                    ScoreTracker.Instance.Add(Data.EventType.Score.EnemyMidAirKill);
                }

                ScoreTracker.Instance.Add(Data.EventType.Score.Kill);
                ScoreTracker.Instance.Add(Data.EventType.Mult.Kill);
            }
        }

        public static bool IsPlayerMoving = false;
        public static bool IsPlayerInAir = false;
        public static bool IsPlayerSeated = false;
        public static bool IsPlayerRagdolled = false;

        private static bool _playerRecentlySwappedAvatars = false;
    }
}
