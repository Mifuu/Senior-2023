using UnityEngine;
using Unity.Netcode;
using System;

public class ElementAttachable : NetworkBehaviour
{
    [SerializeField] private ElementalType initialElement;
    [SerializeField] private bool randomElement;

    public NetworkVariable<ElementalType> networkElement = new NetworkVariable<ElementalType>();

    public override void OnNetworkSpawn()
    {
        networkElement.OnValueChanged += EmitElementChangeEvent;
        if (!IsServer || !randomElement) return;
        networkElement.Value = GetRandomElement();
    }

    public void EmitElementChangeEvent(ElementalType prev, ElementalType current) => ElementChanged?.Invoke(current);
    public ElementalType GetRandomElement() => (ElementalType) UnityEngine.Random.Range(0, Enum.GetNames(typeof(ElementalType)).Length);
    public event Action<ElementalType> ElementChanged;
    public ElementalType element
    {
        get => networkElement.Value;
        set
        {
            if (!IsServer) return;
            networkElement.Value = value;
        }
    }
}
