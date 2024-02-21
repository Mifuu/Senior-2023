using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ObserverPattern
{
    public class EventObservable : IObservable<float>
    {
        public float Value { get => throw new System.NotImplementedException("Do not use"); set => throw new System.NotImplementedException("Do not use"); }
        public event System.Action<float, float> OnValueChanged;
    }
}
