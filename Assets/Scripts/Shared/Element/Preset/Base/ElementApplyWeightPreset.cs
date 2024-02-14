using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ElementApplyWeightPreset", menuName = "Element/Preset/Weapon Weight Preset")]
public class ElementApplyWeightPreset: ScriptableObject
{
    private Dictionary<TemporaryGunType, int> cachedApplyWeightDictionary; 
    [SerializeField] private List<WeaponAndElementalApplyWeight> weightList; 

    public Dictionary<TemporaryGunType, int> TransformListOfWeightToDict(List<WeaponAndElementalApplyWeight> gunTypes)
    {
        Dictionary<TemporaryGunType, int> tempDict = new Dictionary<TemporaryGunType, int>();
        foreach (var gunWeight in gunTypes)
        {
            bool tryAddSuccess = tempDict.TryAdd(gunWeight.gunType, gunWeight.weight);
            if (!tryAddSuccess) Debug.LogWarning("Try adding does not return a successful result");
        }
        return tempDict;
    }

    public Dictionary<TemporaryGunType, int> GetDictionary()
    {
       if (cachedApplyWeightDictionary == null) 
       {
           cachedApplyWeightDictionary = TransformListOfWeightToDict(weightList);
       }
       return cachedApplyWeightDictionary;
    }
}
