using Unity.Netcode;
using UnityEngine.Events;
using UnityEngine;

public interface IDamageable
{
    void Damage(float damageAmount, GameObject dealer);
    void Die(GameObject killer);

    float maxHealth { get; set; }
    NetworkVariable<float> currentHealth { get; set; }
}

