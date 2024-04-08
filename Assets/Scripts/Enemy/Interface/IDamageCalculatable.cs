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
    public ElementalDamageParameter elementalDamageParameter;
    public TemporaryGunType gunType;
    public bool isCrit;

    public DamageInfo(
            GameObject dealer = null,
            float amount = 0f,
            ElementalDamageParameter elementalDamageParameter = new ElementalDamageParameter(),
            TemporaryGunType gunType = TemporaryGunType.None,
            bool isCrit = false)
    {
        this.dealer = dealer;
        this.amount = amount;
        this.elementalDamageParameter = elementalDamageParameter;
        this.gunType = gunType;
        this.isCrit = isCrit;
    }
}

public struct ElementalDamageParameter
{
    public ElementalType element;
    public ElementalEntity elementEntity;

    public ElementalDamageParameter(ElementalType element, ElementalEntity elementalEntity)
    {
        this.element = element;
        this.elementEntity = elementalEntity;
    }
}
