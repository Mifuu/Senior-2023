using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageFloatingSpawner : MonoBehaviour
{
    public GameObject damageFloatingPrefab;

    public void Spawn(string value)
    {
        GameObject damageFloating = Instantiate(damageFloatingPrefab, transform.position, Quaternion.identity);
        damageFloating.GetComponent<DamageFloating>().SetValue(value);
    }
}
