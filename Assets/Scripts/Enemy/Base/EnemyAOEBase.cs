using System.Collections;
using System.Collections.Generic;
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

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
        }

        protected void EmitAOEEndsEvent()
        {
            OnAOEPeriodEnd?.Invoke();
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            ResetValue();
        }

        public virtual void PreEffect() { }
        public virtual void CancelPreEffect() { }
        public virtual void ActivateEffect() { GetComponent<Renderer>().material = activateMaterial; }
        public virtual void CancelEffect() { }
        public virtual void ResetValue()
        {
            this.PlayerTarget = null;
            this.enemy = null;
            GetComponent<Renderer>().material = preActivateMaterial;
        }
    }
}
