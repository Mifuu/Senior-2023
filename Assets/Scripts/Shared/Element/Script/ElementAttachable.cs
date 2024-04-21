using UnityEngine;
using Unity.Netcode;
using System;

public class ElementAttachable : NetworkBehaviour
{
    public event Action<ElementalType> ElementChanged; // Event triggered when element changes

    private ElementalType _element;
    public ElementalType element
    {
        get { return _element; }
        set
        {
            if (_element != value)
            {
                _element = value;
                ElementChanged?.Invoke(value); 
            }
        }
    }
}
