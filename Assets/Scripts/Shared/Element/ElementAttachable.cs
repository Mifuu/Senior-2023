using Unity.Netcode;
using UnityEngine.VFX;
using UnityEngine;
using System;

public class ElementAttachable : NetworkBehaviour
{
    public ElementalType element;
    public VisualEffect vfx;

    public override void OnNetworkSpawn()
    {
        UseRandomElement();
        Vector4 color = vfx.GetVector4("New Color");

        switch (element)
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

    public void Awake()
    {
        vfx = GetComponentInChildren<VisualEffect>();
    }

    public void UseRandomElement()
    {
        element = (ElementalType)UnityEngine.Random.Range(0, Enum.GetValues(typeof(ElementalType)).Length);
    }
}
