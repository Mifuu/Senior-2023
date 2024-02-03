using System;

namespace ObserverPattern
{
    public interface IObservable<T>
    {
        T Value { get; set; }
        event Action<T, T> OnValueChanged;
    }
}
