using UnityEngine;
using Unity.Netcode;

namespace Enemy
{
    public class TossakanArmBased : NetworkBehaviour
    {
        private DamageCalculationComponent damageCalculation;

        public void Initialize(DamageCalculationComponent component)
        {
            damageCalculation = component;
        }

        public void OnTriggerEnter(Collider collider)
        {
            var allDamageable = collider.GetComponentsInChildren<IDamageCalculatable>();
            foreach (var damageable in allDamageable)
            {
                damageable.Damage(damageCalculation.GetFinalDealthDamageInfo());
            }
        }
    }
}
