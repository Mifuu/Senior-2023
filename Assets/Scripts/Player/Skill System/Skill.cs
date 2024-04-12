using Unity.Netcode;
using UnityEngine;

public class Skill : NetworkBehaviour
{
    public string Name { get; private set; }
    public float BaseCooldown { get; private set; }
    public float Cooldown { get; private set; }
    public int ResourceCost { get; private set; }
    public float Multiplier { get; private set; }

    public Skill(string name, float baseCooldown, int resourceCost)
    {
        Name = name;
        BaseCooldown = baseCooldown;
        Cooldown = baseCooldown;
        ResourceCost = resourceCost;
        Multiplier = 1.0f;
    }

    public void SetCooldownMultiplier (float multiplier)
    {
        Multiplier = multiplier;
        SetCooldown();
    }

    private void SetCooldown()
    {
        Cooldown = BaseCooldown * Multiplier;
    }


    public float GetCooldownMultiplier()
    {
        return Multiplier;
    }

    public virtual void Activate()
    {
        Debug.Log($"Activated skill: {Name}");
    }
}
