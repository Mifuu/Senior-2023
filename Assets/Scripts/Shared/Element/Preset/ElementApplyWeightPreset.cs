using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ElementApplyWeightPreset", menuName = "Element/Weapon Weight Preset")]
public class ElementApplyWeightPreset: ScriptableObject
{
    private  Dictionary<TemporaryGunType, int> cachedApplyWeightDictionary; 
    [SerializeField] private List<WeaponAndElementalApplyWeight> weightList; 

    public Dictionary<TemporaryGunType, int> TransformListOfWeightToDict(List<WeaponAndElementalApplyWeight> gunTypes)
    {
        Dictionary<TemporaryGunType, int> tempDict = new Dictionary<TemporaryGunType, int>();
        foreach (var gunWeight in gunTypes)
        {
            tempDict.Add(gunWeight.gunType, gunWeight.weight);
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
