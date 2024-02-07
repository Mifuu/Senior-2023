using Unity.Netcode;
using System.Collections.Generic;
using UnityEngine;

public class ElementAppliable : NetworkBehaviour
{
    private readonly NetworkVariable<ElementalType> currentAppliedElement = new NetworkVariable<ElementalType>(ElementalType.None);
    private ElementalType candidateElement;

    [Header("Preset Configuration")]
    [Tooltip("Setting Preset will override the setting set on individual ElementAppliable Component")]
    [SerializeField] private ElementReactionEffectPreset elementalReactionDictPreset;
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
        if (elementalReactionDictPreset != null)
        {
            effectListDict = elementalReactionDictPreset.GetDictionary();
        }
        else
        {
            effectListDict = elementalReactionDictPreset.TransformListOfEffectToDict(effectList);
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

    public void TryApplyElement(GameObject applier, ElementalDamageParameter elementalDamageParameter, TemporaryGunType gunType)
    {
        int currentGunTypeWeight;
        bool success = applyWeightDict.TryGetValue(gunType, out currentGunTypeWeight);

        if (success)
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
        if (!elementalDamageParameter.elementEntity.ApplyElement(elementalDamageParameter.element)) return;

        if (currentAppliedElement.Value != ElementalType.None)
        {
            if (currentAppliedElement.Value == elementalDamageParameter.element) return;

            elementApplyCount = 0;
            Dictionary<ElementalType, ElementalReactionEffect> initialSearch;
            bool initialSearchSuccess = effectListDict.TryGetValue(currentAppliedElement.Value, out initialSearch);

            if (initialSearchSuccess)
            {
                ElementalReactionEffect secondarySearch;
                bool secondarySearchSuccess = initialSearch.TryGetValue(elementalDamageParameter.element, out secondarySearch);

                if (secondarySearchSuccess)
                    secondarySearch.DoEffect(applier, gameObject);
                else
                    currentAppliedElement.Value = ElementalType.None;
            }

            else
                currentAppliedElement.Value = ElementalType.None;

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
