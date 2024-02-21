using System.Collections.Generic;
using UnityEngine;
using System;

namespace Enemy
{
    [CreateAssetMenu(fileName = "EnemyStatsUpgradeRule", menuName = "Enemy/Enemy Stat/Upgrade Rules")]
    public class EnemyStatUpgradeRulesSO : ScriptableObject
    {
        [SerializeField] public List<EnemyStatUpgradeDetail> upgradeDetails = new List<EnemyStatUpgradeDetail>();

        [Serializable]
        public struct EnemyStatUpgradeDetail
        {
            public enum MethodEnums { Overwrite, LevelBasePolynomial, LevelBaseExponential, LevelBaseSigmoid }
            [SerializeField] public MethodEnums Method;
            [SerializeField] public List<EnemyStat.EnemyStatsEnum> Stats;
            [SerializeField] public float Amount;
            [SerializeField] public int LevelLowerBoundInclusive;
            [SerializeField] public int LevelUpperBoundExclusive;
        }
    }
}
