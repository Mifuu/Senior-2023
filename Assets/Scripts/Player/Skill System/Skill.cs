using Unity.Netcode;
using UnityEngine;

public class Skill : NetworkBehaviour
{
    public string Name { get; private set; }
    public float Cooldown { get; private set; }
    public int ResourceCost { get; private set; }

    public Skill(string name, float cooldown, int resourceCost)
    {
        Name = name;
        Cooldown = cooldown;
        ResourceCost = resourceCost;
    }

    public virtual void Activate()
    {
        Debug.Log($"Activated skill: {Name}");
    }
}
