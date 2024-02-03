using Unity.Netcode;
using UnityEngine;

namespace ObserverPattern
{
    public class NetworkSubject<T> : IObservable<T>
    {
        private NetworkVariable<T> variable_internal;
        public NetworkVariable<T> GetNetworkVariable() => variable_internal;
        public static NetworkSubject<T> ConvertToNetworkSubject(NetworkVariable<T> value) => new NetworkSubject<T>(value);

        private void NetworkOnValueChangeHandler(T prev, T current)
        {
            Value = current;
            OnValueChanged?.Invoke(prev, current);
        }

        public NetworkSubject(NetworkVariable<T> value)
        {
            variable_internal = value;
            variable_internal.OnValueChanged += NetworkOnValueChangeHandler;
        }

        ~NetworkSubject()
        {
            variable_internal.OnValueChanged -= NetworkOnValueChangeHandler;
        }

        public T Value
        {
            get => variable_internal.Value;
            set
            {
                // Note: Use the GetNetworkVariable method and set the value using that instead of setting value using NetworkSubject
                // The reason for this is that this class has no way to know whether the user is a client or a server
                // and setting the NetworkVariable requires the user to be the server.
                Debug.LogError("Error: Do not set the value of NetworkSubject directly, Set the value of NetworkVariable instead.");
            }
        }

        public event System.Action<T, T> OnValueChanged;
    }
}
