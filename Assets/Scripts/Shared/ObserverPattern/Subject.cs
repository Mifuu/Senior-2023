using System;

namespace ObserverPattern
{
    public class Subject<T>: IObservable<T>
    {
        private T _value;

        public Subject(T value)
        {
            Value = value;
        }

        public T Value
        {
            get => _value;
            set
            {
                var oldValue = _value;
                _value = value;
                OnValueChanged?.Invoke(oldValue, value);
            }
        }

        public event Action<T, T> OnValueChanged;
    }
}
