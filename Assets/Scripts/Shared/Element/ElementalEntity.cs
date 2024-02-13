using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class ElementalEntity : NetworkBehaviour
{
    [SerializeField] private float cooldownTime = 5.0f;
    public Dictionary<ElementalType, bool> canApplyElementOfType = new Dictionary<ElementalType, bool>();

    public void Awake()
    {
        foreach (ElementalType reaction in Enum.GetValues(typeof(ElementalType)))
        {
            canApplyElementOfType.TryAdd(reaction, true);
        }
    }

    public bool CheckCanApplyElement(ElementalType elementalType, bool setCooldownIfCanApply)
    {
        bool canApply;
        bool getSuccess = canApplyElementOfType.TryGetValue(elementalType, out canApply);

        if (!setCooldownIfCanApply) return canApply;
        if (canApply && getSuccess)
        {
            StartCoroutine(SetApplyCooldown(elementalType));
            return true;
        }

        return false;
    }

    private IEnumerator SetApplyCooldown(ElementalType elementalType)
    {
        canApplyElementOfType[elementalType] = false;
        yield return new WaitForSeconds(cooldownTime);
        canApplyElementOfType[elementalType] = true;
    }
}
