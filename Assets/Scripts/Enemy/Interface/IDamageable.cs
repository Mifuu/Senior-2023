using Unity.Netcode;
using UnityEngine.Events;

public interface IDamageable
{
    void Damage(float damageAmount);
    void Die();

    float maxHealth { get; set; }
    NetworkVariable<float> currentHealth { get; set; }

    UnityEvent OnHealthChanged { get; set; }
}

