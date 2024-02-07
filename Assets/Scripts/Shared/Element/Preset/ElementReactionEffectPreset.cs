using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName= "ElementReactionEffectPreset", menuName = "Element/Reaction Preset")]
public class ElementReactionEffectPreset: ScriptableObject
{
    private Dictionary<ElementalType, Dictionary<ElementalType, ElementalReactionEffect>> cachedEffectDictionary;
    [SerializeField] private List<ElementalReactionEffect> effectList;

    public Dictionary<ElementalType, Dictionary<ElementalType, ElementalReactionEffect>> TransformListOfEffectToDict(List<ElementalReactionEffect> effectList)
    {
        Dictionary<ElementalType, Dictionary<ElementalType, ElementalReactionEffect>> effectListDict
            = new Dictionary<ElementalType, Dictionary<ElementalType, ElementalReactionEffect>>();

        foreach (var effect in effectList)
        {
            if (!effectListDict.ContainsKey(effect.primary))
            {
                effectListDict.Add(effect.primary, new Dictionary<ElementalType, ElementalReactionEffect>());
            }

            Dictionary<ElementalType, ElementalReactionEffect> innerDict;
            effectListDict.TryGetValue(effect.primary, out innerDict);
            innerDict.TryAdd(effect.secondary, effect);
        }

        return effectListDict;
    }

    public Dictionary<ElementalType, Dictionary<ElementalType, ElementalReactionEffect>> GetDictionary()
    {
        if (cachedEffectDictionary == null)
        {
            cachedEffectDictionary = TransformListOfEffectToDict(effectList);
        }
        return cachedEffectDictionary;
    }
}
