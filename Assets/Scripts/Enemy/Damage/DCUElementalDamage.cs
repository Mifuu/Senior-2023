using UnityEngine;

namespace Enemy
{
    [CreateAssetMenu(menuName = "Enemy/Enemy Damage Unit/Elemental Damage", fileName = "Elemental Damage Calculator")]
    public class DCUElementalDamage : DamageCalculationUnitBase
    {
        public override float CalculateCache(DamageCalculationComponent component, SubscriptionGetter getter, float initialValue)
        {
            float hydro, pyro, electro;
            getter.GetFloat("hydro", out hydro);
            getter.GetFloat("pyro", out pyro);
            getter.GetFloat("electro", out electro);
            return initialValue * hydro * pyro * electro;
        }

        public override void Initialize(DamageCalculationComponent component, SubscriptionAdder adder, bool updateOnChange)
        {
            var enemy = component.gameObject.GetComponent<EnemyBase>();
            var hydro = adder.AddFloat("hydro", enemy.HydroDamageBonus);
            var pyro = adder.AddFloat("pyro", enemy.PyroDamageBonus);
            var electro = adder.AddFloat("electro", enemy.ElectroDamageBonus);

            if (!hydro || !pyro || !electro)
            {
                Debug.LogError("Elemental Damage Calculation Setup Fault");
                // IsEnabled = false;
            }
        }

        public override void Dispose(DamageCalculationComponent component, SubscriptionRemover remover)
        {
            remover.RemoveFloat("hydro");
            remover.RemoveFloat("pyro");
            remover.RemoveFloat("electro");
        }
    }
}
