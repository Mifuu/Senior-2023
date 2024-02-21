using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyRandomPool", menuName = "Enemy/EnemyRandomPool", order = 3)]
public class EnemyRandomPool : ScriptableObject
{
    public List<PoolItem> poolItems;

    public GameObject GetRandomPrefab()
    {
        int totalWeight = 0;
        foreach (PoolItem item in poolItems)
        {
            totalWeight += item.weight;
        }

        int randomWeight = Random.Range(0, totalWeight);
        int currentWeight = 0;
        foreach (PoolItem item in poolItems)
        {
            currentWeight += item.weight;
            if (randomWeight < currentWeight)
            {
                return item.prefab;
            }
        }

        return null;
    }

    [System.Serializable]
    public class PoolItem
    {
        public GameObject prefab;
        public int weight;
    }
}
