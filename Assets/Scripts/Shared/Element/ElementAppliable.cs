using Unity.Netcode;
using System.Collections.Generic;
using UnityEngine;

public class ElementAppliable : NetworkBehaviour
{
    [SerializeField] private ElementalType defaultElement;
    private readonly NetworkVariable<ElementalType> currentAppliedElement = new NetworkVariable<ElementalType>(ElementalType.None);
    private ElementalType candidateElement;

    [Header("Preset Configuration")]
    [Tooltip("Setting Preset will override the setting set on individual ElementAppliable Component")]
    [SerializeField] private ElementReactionEffectPreset elementalReactionEffectPreset;
    [Tooltip("Setting Preset will override the setting set on individual ElementAppliable Component")]
    [SerializeField] private ElementApplyWeightPreset elementalApplyWeightPreset;

    [Header("Elemental Reaction Effect List")]
    [SerializeField] private List<ElementalReactionEffect> effectList = new List<ElementalReactionEffect>();
    private Dictionary<ElementalType, Dictionary<ElementalType, ElementalReactionEffect>> effectListDict
            = new Dictionary<ElementalType, Dictionary<ElementalType, ElementalReactionEffect>>();

    [Header("Weapon Element Apply Weight List")]
    [SerializeField] private List<WeaponAndElementalApplyWeight> applyWeights = new List<WeaponAndElementalApplyWeight>();
    private Dictionary<TemporaryGunType, int> applyWeightDict
        = new Dictionary<TemporaryGunType, int>();

    [Header("Count Until Apply")]
    [SerializeField] private int maxCountToApply = 20;
    [SerializeField] private int defaultWeaponApplyWeight = 1;
    private int elementApplyCount = 0;

    public void Awake()
    {
        if (elementalReactionEffectPreset != null)
        {
            effectListDict = elementalReactionEffectPreset.GetDictionary();
        }
        else
        {
            effectListDict = elementalReactionEffectPreset.TransformListOfEffectToDict(effectList);
        }

        if (elementalApplyWeightPreset != null)
        {
            applyWeightDict = elementalApplyWeightPreset.GetDictionary();
        }
        else
        {
            applyWeightDict = elementalApplyWeightPreset.TransformListOfWeightToDict(applyWeights);
        }
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        currentAppliedElement.Value = defaultElement;
    }

    public void TryApplyElement(GameObject applier, ElementalDamageParameter elementalDamageParameter, TemporaryGunType gunType)
    {
        int currentGunTypeWeight;
        if (!applyWeightDict.TryGetValue(gunType, out currentGunTypeWeight))
        {
            Debug.LogWarning("Gun Elemental Apply Weight not found");
            currentGunTypeWeight = defaultWeaponApplyWeight;
        }

        if (elementalDamageParameter.element != candidateElement)
        {
            elementApplyCount = 0;
            candidateElement = elementalDamageParameter.element;
        }

        elementApplyCount += currentGunTypeWeight;
        if (elementApplyCount <= maxCountToApply) return;

        if (!elementalDamageParameter.elementEntity.CheckCanApplyElement(elementalDamageParameter.element, true)) return;

        if (currentAppliedElement.Value != ElementalType.None)
        {
            if (currentAppliedElement.Value == elementalDamageParameter.element) return;

            elementApplyCount = 0;
            Dictionary<ElementalType, ElementalReactionEffect> initialSearch;

            if (effectListDict.TryGetValue(elementalDamageParameter.element, out initialSearch))
            {
                ElementalReactionEffect secondarySearch;

                if (initialSearch.TryGetValue(currentAppliedElement.Value, out secondarySearch))
                {
                    secondarySearch.DoEffect(applier, gameObject);
                }
                else
                {
                    currentAppliedElement.Value = ElementalType.None;
                }
            }
            else
            {
                currentAppliedElement.Value = ElementalType.None;
            }
            return;
        }

        currentAppliedElement.Value = elementalDamageParameter.element;
    }
}

[System.Serializable]
public struct WeaponAndElementalApplyWeight
{
    [SerializeField] public int weight;
    [SerializeField] public TemporaryGunType gunType;
}

public enum TemporaryGunType
{
    Pistol, Shotgun, RocketLauncher, Rifle, None
}
