using Unity.Netcode;

public interface IDamageable
{
    void Damage(float damageAmount);
    void Die();

    float maxHealth { get; set; }
    NetworkVariable<float> currentHealth { get; set; }
}

