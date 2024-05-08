using UnityEngine;
using Unity.Netcode;

namespace Enemy
{
    public class TossakanArmBased : NetworkBehaviour
    {
        public void Initialize(DamageCalculationComponent component)
        {
            foreach (var trig in GetComponentsInChildren<TossakanArmBasedTrigger>())
            {
                trig.InjectComponent(component);
            }
        }
    }
}
