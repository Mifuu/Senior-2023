using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

namespace Enemy
{
    public class EnemyHitbox : MonoBehaviour, ITriggerCheckable
    {
        private GameObject PlayerTarget { get; set; }
        private Enemy.EnemyBase _enemy;

        // TODO: Check if hashset is working
        // BUG => Migrating to hashset should fix this: Adding same element (such as the same player) is allowed
        public HashSet<GameObject> ObjectsInTrigger { get; set; }
        public HashSet<GameObject> PlayerWithinTrigger { get; set; }
        public event Action<Collider> OnHitboxTriggerEnter;
        public event Action<Collider> OnHitboxTriggerExit;

        private void Awake()
        {
            PlayerWithinTrigger = new HashSet<GameObject>();
            ObjectsInTrigger = new HashSet<GameObject>();
            PlayerTarget = GameObject.FindGameObjectWithTag("Player");
            _enemy = GetComponentInParent<Enemy.EnemyBase>();
        }

        public virtual void OnTriggerEnter(Collider collider)
        {
            OnHitboxTriggerEnter?.Invoke(collider);
            ObjectsInTrigger.Add(collider.gameObject);
            if (collider.CompareTag("Player"))
            {
                PlayerWithinTrigger.Add(collider.gameObject);
            }
        }

        public virtual void OnTriggerExit(Collider collider)
        {
            OnHitboxTriggerExit?.Invoke(collider);
            ObjectsInTrigger.Remove(collider.gameObject);
            if (collider.CompareTag("Player"))
            {
                PlayerWithinTrigger.Remove(collider.gameObject);
            }
        }
    }
}
