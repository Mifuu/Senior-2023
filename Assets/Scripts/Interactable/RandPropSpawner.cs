using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class RandPropSpawner : MonoBehaviour
{
    public PropItem[] propItems;
    public bool spawnOnStart = true;

    void Start()
    {
        if (spawnOnStart)
        {
            SpawnProp();
        }
    }

    void SpawnProp()
    {
        int poolSize = 0;
        foreach (PropItem propItem in propItems)
            poolSize += propItem.amount;

        int poolIndex = Random.Range(0, poolSize);

        foreach (PropItem propItem in propItems)
        {
            poolIndex -= propItem.amount;

            if (poolIndex < 0)
            {
                if (propItem.prefab == null)
                    return;
                GameObject prop = Instantiate(propItem.prefab, transform.position, transform.rotation);
                NetworkObject networkObject = prop.GetComponent<NetworkObject>();
                if (networkObject != null)
                {
                    networkObject.Spawn();
                }
                break;
            }
        }
    }

    [System.Serializable]
    public class PropItem
    {
        public GameObject prefab;
        public int amount;
    }
}
