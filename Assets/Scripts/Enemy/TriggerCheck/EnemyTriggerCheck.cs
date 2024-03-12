using UnityEngine;
using Unity.Netcode;
using System;

namespace Enemy
{
    public class EnemyTriggerCheck : NetworkBehaviour
    {
        private Enemy.EnemyBase _enemy;
        public event Action<Collider> OnHitboxTriggerEnter;
        public event Action<Collider> OnHitboxTriggerExit;

        public virtual void Awake()
        {
            _enemy = GetComponentInParent<EnemyBase>();
        }

        public virtual void OnTriggerEnter(Collider collider)
        {
            OnHitboxTriggerEnter?.Invoke(collider);
        }

        public virtual void OnTriggerExit(Collider collider)
        {
            OnHitboxTriggerExit?.Invoke(collider);
        }
    }
}
