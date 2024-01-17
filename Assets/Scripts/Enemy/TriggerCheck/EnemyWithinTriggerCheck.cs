using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class EnemyWithinTriggerCheck : EnemyTriggerCheck
    {
        public HashSet<GameObject> ObjectsInTrigger { get; set; }
        public HashSet<GameObject> PlayerWithinTrigger { get; set; }

        public override void Awake()
        {
            base.Awake();
            PlayerWithinTrigger = new HashSet<GameObject>();
            ObjectsInTrigger = new HashSet<GameObject>();
        }

        public override void OnTriggerEnter(Collider collider)
        {
            base.OnTriggerEnter(collider);
            ObjectsInTrigger.Add(collider.gameObject);
            if (collider.CompareTag("Player"))
            {
                PlayerWithinTrigger.Add(collider.gameObject);
            }
        }

        public override void OnTriggerExit(Collider collider)
        {
            base.OnTriggerExit(collider);
            ObjectsInTrigger.Remove(collider.gameObject);
            if (collider.CompareTag("Player"))
            {
                PlayerWithinTrigger.Remove(collider.gameObject);
            }
        }
    }
}
