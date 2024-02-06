using UnityEngine;
using ObserverPattern;

namespace Enemy
{
    [CreateAssetMenu(menuName = "Enemy/Enemy Damage Unit/Crit Calculator", fileName = "Crit Calculation Unit")]
    public class DCUCrit : DamageCalculationUnit<float>
    {
        // Non Static Unit
        EnemyBase enemy;
        Subject<float> CritDmgBonus;
        Subject<float> CritRate;

        public override void Setup()
        {
            enemy = gameObject.GetComponent<EnemyBase>();
            CritDmgBonus = enemy.CritDmgFactor;
            CritRate = enemy.CritRate;

            if (CritRate == null || CritDmgBonus == null)
            {
                Debug.LogError(gameObject + " Crit Damage Calculation Setup Fault");
                IsEnabled = false;
            }
        }

        public override DamageInfo Calculate(DamageInfo info)
        {
            var randomNum = Random.Range(0f, 1f);
            if (randomNum <= CritRate.Value)
            {
                info.amount *= CritDmgBonus.Value;
            }

            return info;
        }
    }
}
