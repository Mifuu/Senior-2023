using Unity.Netcode;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

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

    private VisualEffect vfx;

    public void Awake()
    {
        if (elementalReactionEffectPreset != null)
        {
            effectListDict = elementalReactionEffectPreset.GetDictionary();
        }
        else
        {
            Debug.LogWarning("This might not work currently");
            effectListDict = elementalReactionEffectPreset.TransformListOfEffectToDict(effectList);
        }

        if (elementalApplyWeightPreset != null)
        {
            applyWeightDict = elementalApplyWeightPreset.GetDictionary();
        }
        else
        {
            Debug.LogWarning("This might not work currently");
            applyWeightDict = elementalApplyWeightPreset.TransformListOfWeightToDict(applyWeights);
        }

        vfx = GetComponentInChildren<VisualEffect>();
    }

    public override void OnNetworkSpawn()
    {
        currentAppliedElement.OnValueChanged += ChangeVfxColorOfAppliedElement;
        if (!IsServer) return;
        currentAppliedElement.Value = defaultElement;
    }

    public override void OnNetworkDespawn()
    {
        currentAppliedElement.OnValueChanged -= ChangeVfxColorOfAppliedElement;
    }

    public void ChangeVfxColorOfAppliedElement(ElementalType prev, ElementalType current)
    {
        if (vfx == null) return;
        Vector4 color = vfx.GetVector4("New Color");

        switch (currentAppliedElement.Value)
        {
            case ElementalType.None:
                color = new Vector4(147, 147, 147, 231);
                break;
            case ElementalType.Water:
                color = new Vector4(7, 152, 191, 255);
                break;
            case ElementalType.Fire:
                color = new Vector4(191, 63, 0, 231);
                break;
            case ElementalType.Earth:
                color = new Vector4(191, 2, 0, 231);
                break;
            case ElementalType.Wind:
                color = new Vector4(0, 191, 21, 231);
                break;
        }

        vfx.SetVector4("New Color", color);
    }

    public void TryApplyElement(GameObject applier, ElementalDamageParameter elementalDamageParameter, TemporaryGunType gunType)
    {
        int currentGunTypeWeight;
        if (!applyWeightDict.TryGetValue(gunType, out currentGunTypeWeight))
        {
            // Debug.LogWarning("Gun Elemental Apply Weight not found");
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
