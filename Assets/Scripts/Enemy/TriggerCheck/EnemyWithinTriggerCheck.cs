using System.Collections.Generic;
using UnityEngine;
using System;

namespace Enemy
{
    public class EnemyWithinTriggerCheck : EnemyTriggerCheck
    {
        [SerializeField] private bool onlyCountPlayer = false;
        public HashSet<GameObject> ObjectsInTrigger { get; set; }
        public HashSet<GameObject> PlayerWithinTrigger { get; set; }

        public event Action<GameObject, int> OnPlayerEnteringTrigger;
        public event Action<GameObject, int> OnPlayerLeavingTrigger;

        public event Action<GameObject, int> OnObjectEnteringTrigger;
        public event Action<GameObject, int> OnObjectLeavingTrigger;

        public override void Awake()
        {
            base.Awake();
            PlayerWithinTrigger = new HashSet<GameObject>();
            ObjectsInTrigger = new HashSet<GameObject>();
        }

        public override void OnTriggerEnter(Collider collider)
        {
            base.OnTriggerEnter(collider);

            if (!onlyCountPlayer)
            {
                ObjectsInTrigger.Add(collider.gameObject);
                OnObjectEnteringTrigger?.Invoke(collider.gameObject, ObjectsInTrigger.Count);
            }

            if (collider.CompareTag("Player"))
            {
                PlayerWithinTrigger.Add(collider.gameObject);
                OnPlayerEnteringTrigger?.Invoke(collider.gameObject, PlayerWithinTrigger.Count);
            }
        }

        public override void OnTriggerExit(Collider collider)
        {
            base.OnTriggerExit(collider);

            if (!onlyCountPlayer)
            {
                ObjectsInTrigger.Remove(collider.gameObject);
                OnObjectLeavingTrigger?.Invoke(collider.gameObject, ObjectsInTrigger.Count);
            }

            if (collider.CompareTag("Player"))
            {
                PlayerWithinTrigger.Remove(collider.gameObject);
                OnPlayerLeavingTrigger?.Invoke(collider.gameObject, PlayerWithinTrigger.Count);
            }
        }
    }
}
