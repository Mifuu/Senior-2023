using System;

namespace ObserverPattern
{
    public class Subject<T>: IObservable<T>
    {
        public Subject(T value)
        {
            Value = value;
        }

        public T Value
        {
            get => Value;
            set
            {
                var oldValue = Value;
                Value = value;
                OnValueChanged?.Invoke(oldValue, value);
            }
        }

        public event Action<T, T> OnValueChanged;
    }
}
