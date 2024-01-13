using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace Enemy
{
    // TODO: Add IPlayerTrackable
    public abstract class EnemyAOEBase : NetworkBehaviour
    {
        [SerializeField] protected GameObject AOEGameObjectPrefab;
        public GameObject PlayerTarget { get; set; }
        public EnemyBase enemy { get; set; }
        protected EnemyHitbox areaOfEffectTrigger;

        public void Awake()
        {
            areaOfEffectTrigger = transform.Find("AOE")?.GetComponent<EnemyHitbox>();
            if (areaOfEffectTrigger == null)
            {
                Debug.LogError("AOE have no trigger check (Hitbox)");
            }
        }

        public virtual void ActivateAOE(GameObject PlayerTarget, EnemyBase enemy)
        {
            this.PlayerTarget = PlayerTarget;
            this.enemy = enemy;
        }

        public override void OnNetworkDespawn()
        {
            ResetValue();
        }

        public virtual void PreEffect() { }
        public virtual void CancelPreEffect() { }
        public abstract void ActivateEffect();
        public abstract void CancelEffect();
        public virtual void ResetValue()
        {
            this.PlayerTarget = null;
            this.enemy = null;
        }
    }
}
