using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Unity.Netcode;
using ObserverPattern;
using System;

namespace Enemy
{
    public class EnemyStat : NetworkBehaviour
    {
        public enum EnemyStatsEnum { MaxHP, BaseATK, BaseDEF, ElementDMG, ElementalRES, BaseEXP }

        [Header("Level Stats Detail")]
        [SerializeField] private EnemyStatUpgradeRulesSO upgradeDetail;
        [SerializeField] private EnemyGunWeaknessSO gunWeakness;

        [Header("Default Stats on LV1")]
        [SerializeField] private float defaultBaseDMG = 1.0f;
        [SerializeField] private float defaultBaseDMGReceiveFactor = 1.0f;
        [SerializeField] private float defaultElementalDmg = 1.0f;
        [SerializeField] private float defaultElementalRES = 1.0f;
        [SerializeField] private float defaultBaseEXP = 20;

        [HideInInspector] public NetworkVariable<int> Level = new NetworkVariable<int>(1);
        public Subject<float> BaseDamage = new Subject<float>(5.0f);
        public Subject<float> DamageReceiveFactor = new Subject<float>(1.0f);
        public Subject<float> BaseEXP = new Subject<float>(0);
        public List<Subject<float>> ListOfElementalDamageBonus = new List<Subject<float>>();
        public List<Subject<float>> ListOfElementalRES = new List<Subject<float>>();

        private EnemyBase enemy;

        public void Awake()
        {
            BaseDamage.Value = defaultBaseDMG;
            DamageReceiveFactor.Value = defaultBaseDMGReceiveFactor;
            BaseEXP.Value = defaultBaseEXP;
            for (int i = 0; i < Enum.GetNames(typeof(ElementalType)).Length; i++)
            {
                ListOfElementalDamageBonus.Add(new Subject<float>(defaultElementalDmg));
                ListOfElementalRES.Add(new Subject<float>(defaultElementalRES));
            }

            enemy = GetComponent<EnemyBase>();
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            Level.OnValueChanged += SetStatOnCurrentLevel;
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            Level.OnValueChanged -= SetStatOnCurrentLevel;
            ResetStat();
        }

        public void ResetStat()
        {
            BaseDamage.Value = defaultBaseDMG;
            DamageReceiveFactor.Value = defaultBaseDMGReceiveFactor;
            for (int i = 0; i < Enum.GetNames(typeof(ElementalType)).Length; i++)
            {
                ListOfElementalDamageBonus[i].Value = defaultElementalDmg;
                ListOfElementalRES[i].Value = defaultElementalRES;
            }
        }

        private void SetStatOnCurrentLevel(int prev, int current)
        {
            void StatsReducerFunction(List<EnemyStatsEnum> stats, float value, EnemyStatUpgradeRulesSO.EnemyStatUpgradeDetail.MethodEnums method)
            {
                Action<Subject<float>, float> Reducer(EnemyStatUpgradeRulesSO.EnemyStatUpgradeDetail.MethodEnums method)
                {
                    switch (method)
                    {
                        case EnemyStatUpgradeRulesSO.EnemyStatUpgradeDetail.MethodEnums.LevelBasePolynomial:
                            return (Subject<float> subject, float value) =>
                            {
                                subject.Value = subject.Value + (value * current);
                            };
                        case EnemyStatUpgradeRulesSO.EnemyStatUpgradeDetail.MethodEnums.LevelBaseExponential:
                            return (Subject<float> subject, float value) =>
                            {
                                subject.Value = subject.Value * (float)Math.Pow(value, current);
                            };
                        case EnemyStatUpgradeRulesSO.EnemyStatUpgradeDetail.MethodEnums.LevelBaseSigmoid:
                            return (Subject<float> subject, float value) =>
                            {
                                subject.Value = subject.Value / (1.0f + (float)Math.Exp(-subject.Value));
                            };
                        case EnemyStatUpgradeRulesSO.EnemyStatUpgradeDetail.MethodEnums.Overwrite:
                            return (Subject<float> subject, float value) =>
                            {
                                subject.Value = value;
                            };
                        default:
                            return (Subject<float> subject, float value) => { };
                    }
                }

                var reducer = Reducer(method);
                stats.ForEach((stat) =>
                {
                    switch (stat)
                    {
                        case EnemyStatsEnum.BaseATK:
                            reducer(BaseDamage, value);
                            break;
                        case EnemyStatsEnum.BaseDEF:
                            reducer(DamageReceiveFactor, value);
                            break;
                        case EnemyStatsEnum.BaseEXP:
                            reducer(BaseEXP, value);
                            break;
                        case EnemyStatsEnum.ElementDMG:
                            ListOfElementalDamageBonus.ForEach((elementStat) =>
                            {
                                reducer(elementStat, value);
                            });
                            break;
                        case EnemyStatsEnum.ElementalRES:
                            ListOfElementalRES.ForEach((elementStat) =>
                            {
                                reducer(elementStat, value);
                            });
                            break;
                        case EnemyStatsEnum.MaxHP:
                            Subject<float> temp = new Subject<float>(enemy.maxHealth);
                            reducer(temp, value);
                            enemy.ChangeMaxHealthClientRpc(temp.Value);
                            break;
                    }
                });
            }

            upgradeDetail.upgradeDetails
                .FindAll((detail) => current >= detail.LevelLowerBoundInclusive
                        && current < detail.LevelUpperBoundExclusive).ForEach((detail) =>
                            {
                                StatsReducerFunction(detail.Stats, detail.Amount, detail.Method);
                            });
        }

        public void SetEnemyLevel(int level)
        {
            if (!IsServer) return;
            Level.Value = level;
        }

        public Subject<float> GetElementalDMGSubject(ElementalType type) => ListOfElementalDamageBonus[(int)type];
        public Subject<float> GetElementalRESSubject(ElementalType type) => ListOfElementalRES[(int)type];
        public float GetElementalDMGValue(ElementalType type) => GetElementalDMGSubject(type).Value;
        public float GetElementalRESValue(ElementalType type) => GetElementalRESSubject(type).Value;

        public IEnumerator CreateDelayFunctionCall(Action func, float time)
        {
            yield return new WaitForSeconds(time);
            func();
        }

        private Action BuffAndGenerateDebuffAction(Subject<float> stat, float add, float multiply)
        {
            float previousStatValue = stat.Value;
            stat.Value = (stat.Value + add) * multiply;
            return () =>
            {
                stat.Value = previousStatValue;
            };
        }

        private Action BuffAndGenerateDebuffActionForList(List<Subject<float>> stats, int index, float add, float multiply)
        {
            float previousStatValue = stats[index].Value;
            stats[index].Value = (stats[index].Value + add) * multiply;
            return () =>
            {
                stats[index].Value = previousStatValue;
            };
        }

        [ServerRpc]
        public void BuffATKServerRpc(float time = 0.0f, float add = 0.0f, float multiply = 1.0f)
        {
            if (time > 0.01f)
            {
                StartCoroutine(CreateDelayFunctionCall(BuffAndGenerateDebuffAction(BaseDamage, add, multiply), time));
                return;
            }
            BuffAndGenerateDebuffAction(BaseDamage, add, multiply);
        }

        [ServerRpc]
        public void BuffDEFServerRpc(float time = 0.0f, float add = 0.0f, float multiply = 1.0f)
        {
            if (time > 0.01f)
            {
                StartCoroutine(CreateDelayFunctionCall(BuffAndGenerateDebuffAction(DamageReceiveFactor, add, multiply), time));
                return;
            }
            BuffAndGenerateDebuffAction(DamageReceiveFactor, add, multiply);
        }

        [ServerRpc]
        public void BuffElementDMGServerRpc(ElementalType element, float time = 0.0f, float add = 0.0f, float multiply = 1.0f)
        {
            if (time > 0.01f)
            {
                StartCoroutine(CreateDelayFunctionCall(BuffAndGenerateDebuffActionForList(ListOfElementalDamageBonus, (int)element, add, multiply), time));
                return;
            }
            BuffAndGenerateDebuffActionForList(ListOfElementalDamageBonus, (int)element, add, multiply);
        }

        [ServerRpc]
        public void BuffElementRESServerRpc(ElementalType element, float time = 0.0f, float add = 0.0f, float multiply = 1.0f)
        {
            if (time > 0.01f)
            {
                StartCoroutine(CreateDelayFunctionCall(BuffAndGenerateDebuffActionForList(ListOfElementalRES, (int)element, add, multiply), time));
                return;
            }
            BuffAndGenerateDebuffActionForList(ListOfElementalRES, (int)element, add, multiply);
        }
    }
}
