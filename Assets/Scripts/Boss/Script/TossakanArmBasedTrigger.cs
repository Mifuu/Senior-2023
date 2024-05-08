using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class TossakanArmBasedTrigger : NetworkBehaviour
{
    private DamageCalculationComponent damageCalculation;

    public void InjectComponent(DamageCalculationComponent component)
    {
        damageCalculation = component;
    }

    public void OnTriggerEnter(Collider collider)
    {
        Debug.Log("Collide with " + collider.gameObject);
        var allDamageable = collider.GetComponentsInChildren<IDamageCalculatable>();
        foreach (var damageable in allDamageable)
        {
            damageable.Damage(damageCalculation.GetFinalDealthDamageInfo());
        }
    }
}
