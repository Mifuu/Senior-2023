using UnityEngine;
using Unity.Netcode;
using System;

namespace Enemy
{
    public abstract class EnemyAOEBase : NetworkBehaviour
    {
        [SerializeField] protected GameObject AOEGameObjectPrefab;
        public GameObject PlayerTarget { get; set; }
        public EnemyBase enemy { get; set; }

        [Header("Temporary Material Field")]
        [SerializeField] private Material activateMaterial;
        [SerializeField] private Material preActivateMaterial;
        private NetworkVariable<bool> isOnActivateMaterial = new NetworkVariable<bool>(false);

        protected EnemyWithinTriggerCheck areaOfEffectTrigger;
        public event Action OnAOEPeriodEnd;

        public void Awake()
        {
            areaOfEffectTrigger = transform.Find("AOE")?.GetComponent<EnemyWithinTriggerCheck>();
            if (areaOfEffectTrigger == null)
            {
                Debug.LogError("AOE have no trigger check (Hitbox)");
            }
        }

        public virtual void InitializeAOE(GameObject PlayerTarget, EnemyBase enemy)
        {
            Debug.Log("AOE Base Player Target: " + PlayerTarget);
            this.PlayerTarget = PlayerTarget;
            this.enemy = enemy;
        }

        protected void EmitAOEEndsEvent()
        {
            OnAOEPeriodEnd?.Invoke();
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            isOnActivateMaterial.OnValueChanged += ChangeMaterial;
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            ResetValue();
            isOnActivateMaterial.OnValueChanged -= ChangeMaterial;
        }

        private void ChangeMaterial(bool _, bool current)
        {
            if (current)
            {
                GetComponent<Renderer>().material = activateMaterial;
            }
            else
            {
                GetComponent<Renderer>().material = preActivateMaterial;
            }
        }

        public virtual void PreEffect() { }
        public virtual void CancelPreEffect() { }
        public virtual void ActivateEffect()
        {
            if (!IsServer) return;
            isOnActivateMaterial.Value = true;
        }
        public virtual void CancelEffect() { }
        public virtual void ResetValue()
        {
            this.PlayerTarget = null;
            this.enemy = null;
            if (!IsServer) return;
            isOnActivateMaterial.Value = false;
        }
    }
}
