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
        public event Action OnStatChanged;

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
            // OnStatChanged += OnStatChangeDebug;
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            Level.OnValueChanged -= SetStatOnCurrentLevel;
            Level.Value = 1;
            // OnStatChanged -= OnStatChangeDebug;
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

        private void OnStatChangeDebug()
        {
            Debug.Log($"BaseATK: {BaseDamage.Value}");
            Debug.Log($"BaseDef: {DamageReceiveFactor.Value}");
            Debug.Log($"BaseEXP: {BaseEXP.Value}");
            Debug.Log($"MaxHP: {enemy.networkMaxHealth.Value}");
        }

        private void SetStatOnCurrentLevel(int prev, int current)
        {
            if (prev > current) 
            {
                Debug.LogError("Does not support downgrading, Aborting...");
                return;                
            }

            foreach (var detail in upgradeDetail.upgradeDetails)
            {
                if (prev > detail.LevelUpperBoundExclusive || current < detail.LevelLowerBoundInclusive)
                    continue;

                int lower = prev, upper = current;
                if (detail.LevelLowerBoundInclusive > prev)
                    lower = detail.LevelLowerBoundInclusive;
                if (detail.LevelUpperBoundExclusive < current)
                    upper = detail.LevelUpperBoundExclusive;

                int upgradeLevel = upper - lower;

                float UpgradeStatValueReducer(float initialValue)
                {
                    switch (detail.Method)
                    {
                        case EnemyStatUpgradeRulesSO.EnemyStatUpgradeDetail.MethodEnums.Overwrite:
                            return detail.Amount;
                        case EnemyStatUpgradeRulesSO.EnemyStatUpgradeDetail.MethodEnums.Additive:
                            return initialValue + (detail.Amount * upgradeLevel);
                        case EnemyStatUpgradeRulesSO.EnemyStatUpgradeDetail.MethodEnums.Multiplicative:
                            return initialValue * ((float)Math.Pow(detail.Amount, upgradeLevel));
                        default:
                            throw new System.InvalidOperationException("Enemy Upgrade Method Not Allowed");
                    }
                }

                foreach (var stat in detail.Stats)
                {
                    switch (stat)
                    {
                        case EnemyStatsEnum.MaxHP:
                            enemy.networkMaxHealth.Value = UpgradeStatValueReducer(enemy.networkMaxHealth.Value);
                            break;
                        case EnemyStatsEnum.BaseATK:
                            BaseDamage.Value = UpgradeStatValueReducer(BaseDamage.Value);
                            break;
                        case EnemyStatsEnum.BaseDEF:
                            DamageReceiveFactor.Value = UpgradeStatValueReducer(DamageReceiveFactor.Value);
                            break;
                        case EnemyStatsEnum.BaseEXP:
                            BaseEXP.Value = UpgradeStatValueReducer(BaseEXP.Value);
                            break;
                        case EnemyStatsEnum.ElementDMG:
                            foreach (var el in ListOfElementalDamageBonus)
                                el.Value = UpgradeStatValueReducer(el.Value);
                            break;
                        case EnemyStatsEnum.ElementalRES:
                            foreach (var el in ListOfElementalRES)
                                el.Value = UpgradeStatValueReducer(el.Value);
                            break;
                    }
                }
            }

            OnStatChanged?.Invoke();
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
