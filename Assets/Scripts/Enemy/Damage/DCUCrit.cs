using UnityEngine;
using ObserverPattern;

namespace Enemy
{
    [CreateAssetMenu(menuName = "Enemy/Enemy Damage Unit/Crit Calculator", fileName = "Crit Calculation Unit")]
    public class DCUCrit : DamageCalculationUnitBase
    {
        Subject<float> CritRate;
        Subject<float> CritDmgBonus;

        public override DamageInfo CalculateActual(DamageCalculationComponent component, SubscriptionGetter getter, DamageInfo info)
        {
            var randomNum = Random.Range(0f, 1f);
            if (randomNum <= CritRate.Value)
            {
                info.amount *= CritDmgBonus.Value;
            }

            Debug.Log("Actual info: " + info.amount);
            return info;
        }

        public override void Initialize(DamageCalculationComponent component, SubscriptionAdder adder, bool updateOnChange)
        {
            var enemy = component.gameObject.GetComponent<EnemyBase>();
            CritDmgBonus = enemy.CritDmgFactor;
            CritRate = enemy.CritRate;

            if (CritRate == null || CritDmgBonus == null)
            {
                Debug.LogError(component.gameObject + " Crit Damage Calculation Setup Fault");
                // IsEnabled = false;
            }
        }

        public override void Dispose(DamageCalculationComponent component, SubscriptionRemover remover)
        {
            throw new System.NotImplementedException();
        }
    }
}
