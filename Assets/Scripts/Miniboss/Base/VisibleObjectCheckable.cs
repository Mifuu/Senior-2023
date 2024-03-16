using UnityEngine;
using ObserverPattern;
using Unity.Netcode;

namespace Enemy
{
    public class VisibleObjectCheckable : NetworkBehaviour
    {
        [Header("Activation Option")]
        [Tooltip("Set the object as visible the moment it is being seen, instead of using a counter")]
        [SerializeField] private bool useSubsequentIncrease = true;
        [Tooltip("Set the object as invisible the moment it is being seen, instead of using a counter")]
        [SerializeField] private bool useSubsequentDecrease = true;
        [Tooltip("Set this to false and this component will not do anything")]
        public bool isActivated = true;

        [Header("Visible Counter Option")]

        [Tooltip("If the visible counter reached this threshold, it is considered visible")]
        [Min(0)]
        [SerializeField] private int visibleThreshold = 5;

        [Tooltip("Visible counter will only increment up to this value")]
        [Min(0)]
        [SerializeField] private int visibleCounterMaxValue = 8;

        [Tooltip("Interval in seconds that the visible counter is incremented")]
        [Min(0)]
        [SerializeField] private float visibleCountIncrementInterval = 1;

        [Min(0)]
        [Tooltip("Interval in seconds that the visible counter is decremented")]
        [SerializeField] private float visibleCountDecrementInterval = 1;

        public Subject<int> visibleCounter = new Subject<int>(0);
        public Subject<bool> isVisible = new Subject<bool>(false);
        public Subject<bool> rawIsVisible = new Subject<bool>(false);

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            visibleCounter.OnValueChanged += DebugPrintCurrentCounter;
            visibleCounter.OnValueChanged += DetermineThreshold;
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            isActivated = false;
            isVisible.Value = false;
            visibleCounter.Value = 0;
            visibleCounter.OnValueChanged -= DebugPrintCurrentCounter;
            visibleCounter.OnValueChanged -= DetermineThreshold;
        }

        private void DebugPrintCurrentCounter(int prev, int current)
        {
            Debug.Log("Current Counter: " + current);
        }

        public void DetermineThreshold(int prev, int current)
        {
            if (current >= visibleThreshold && !isVisible.Value)
                isVisible.Value = true;
            if (current < visibleThreshold && isVisible.Value)
                isVisible.Value = false;
        }

        public void OnBecameVisible() // Unity builtin function
        {
            Debug.LogWarning("VIsible");
            rawIsVisible.Value = true;
            if (!isActivated) return;

            if (useSubsequentDecrease)
                CancelInvoke("DecreaseCounter");

            if (useSubsequentIncrease)
                InvokeRepeating("IncreaseCounter", 0f, visibleCountIncrementInterval);
            else
            {
                isVisible.Value = true;
                visibleCounter.Value = visibleThreshold;
            }
        }

        public void OnBecameInvisible() // Unity builtin function
        {
            Debug.LogWarning("Invisible");
            rawIsVisible.Value = false;
            if (!isActivated) return;

            if (useSubsequentIncrease)
                CancelInvoke("IncreaseCounter");

            if (useSubsequentDecrease)
                InvokeRepeating("DecreaseCounter", 0f, visibleCountDecrementInterval);
            else
            {
                isVisible.Value = false;
                visibleCounter.Value = 0;
            }
        }

        private void IncreaseCounter()
        {
            if (visibleCounter.Value >= visibleCounterMaxValue) return;
            visibleCounter.Value = visibleCounter.Value + 1;
        }

        private void DecreaseCounter()
        {
            if (visibleCounter.Value == 0) return;
            visibleCounter.Value = visibleCounter.Value - 1;
        }
    }
}
