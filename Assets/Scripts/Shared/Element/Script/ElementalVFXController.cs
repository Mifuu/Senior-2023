using UnityEngine;
using Unity.Netcode;
using UnityEngine.VFX;
using System;
using System.Collections;

public class ElementalVFXController : NetworkBehaviour
{
    private const string spawnRateSettingName = "SuperSpawn";

    [Header("Particle Spawn Setting")]
    [SerializeField] private float superSpawnTime;

    [SerializeField] private ElementalVFXMap[] vfxMaps;
    [SerializeField] private ElementAppliable appliable;

    public void Awake()
    {
        if (appliable == null) 
        {
            Debug.LogError("Unassigned Element Appliable: Please Assign Appliable for the Elemental VFX to work");
            enabled = false;
            return;
        }
        for (int i = 0; i < vfxMaps.Length; i++)
            vfxMaps[i].VFX.Stop();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        for (int i = 0; i < vfxMaps.Length; i++)
            vfxMaps[i].VFX.Stop();
        CheckAndApplyElementalVFX(ElementalType.None, appliable.currentAppliedElement.Value);
        appliable.currentAppliedElement.OnValueChanged += CheckAndApplyElementalVFX;
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        appliable.currentAppliedElement.OnValueChanged -= CheckAndApplyElementalVFX;
    }

    public VisualEffect GetVFX(ElementalType element)
    {
        foreach (var map in vfxMaps)
            if (map.element == element) return map.VFX;
        return null;
    }

    private void CheckAndApplyElementalVFX(ElementalType prev, ElementalType current)
    {
        for (int i = 0; i < vfxMaps.Length; i++)
        {
            var currentVfxMap = vfxMaps[i];
            if (currentVfxMap.element == current)
                currentVfxMap.VFX.Play();
            else
            {
                StartCoroutine(SuperSpawnAndStop(currentVfxMap.VFX));
            }
        }
    }

    private IEnumerator SuperSpawnAndStop(VisualEffect vfx)
    {
        vfx.SetBool(spawnRateSettingName, true);
        yield return new WaitForSeconds(superSpawnTime);
        vfx.SetBool(spawnRateSettingName, false);
        vfx.Stop();
    }
}

[Serializable]
public struct ElementalVFXMap
{
    [SerializeField] public ElementalType element;
    [SerializeField] public VisualEffect VFX;
}
