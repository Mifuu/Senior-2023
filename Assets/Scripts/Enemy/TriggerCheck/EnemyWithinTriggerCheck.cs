using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Netcode;

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
                if (collider.TryGetComponent<PlayerHealth>(out var health) && collider.TryGetComponent<NetworkObject>(out var networkObject))
                    health.OnPlayerDie += CheckPlayerDied(networkObject);
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

        public Action CheckPlayerDied(NetworkObject networkObject)
        {
            return () =>
            {
                foreach (var obj in PlayerWithinTrigger)
                {
                    if (obj.TryGetComponent<NetworkObject>(out var nobj))
                    {
                        if (nobj.NetworkObjectId == networkObject.NetworkObjectId)
                        {
                            PlayerWithinTrigger.Remove(obj);
                            break;
                        }
                    }
                }
            };
        }
    }
}
