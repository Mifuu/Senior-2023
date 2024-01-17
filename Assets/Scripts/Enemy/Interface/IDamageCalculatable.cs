using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// <summary>
// This interface can be used in the hitbox out of enemy and player
// Allows for API to Damage the damageable with the correct parameter 
// </summary>
public interface IDamageCalculatable
{
    void Damage(DamageInfo damageInfo);
    float getCurrentHealth();
}

// <summary>
// DamageInfo struct is used to describe the damage
// to be given to other damagecalculatable
// </summary>
public struct DamageInfo
{
    public GameObject dealer;
    public float amount;
    // GunType?
    // Element?
    // BuffEffect?

    public DamageInfo(GameObject dealer, float amount)
    {
        this.dealer = dealer;
        this.amount = amount;
    }
}
