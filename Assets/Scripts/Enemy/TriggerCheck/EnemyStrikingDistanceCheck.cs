using UnityEngine;
using Unity.Netcode;

namespace Enemy
{
    public class EnemyStrikingDistanceCheck : NetworkBehaviour
    {
        private GameObject PlayerTarget { get; set; }
        private Enemy.EnemyBase _enemy;

        public void Awake()
        {
            // TODO: Change this PlayerTarget to reflect the correct Player
            PlayerTarget = GameObject.FindGameObjectWithTag("Player");
            _enemy = GetComponentInParent<EnemyBase>();
        }

        private void OnTriggerEnter(Collider collider)
        {
            if (!IsServer) return;
            if (collider.gameObject == PlayerTarget)
            {
                _enemy.SetStrikingDistanceBool(true);
            }
        }

        private void OnTriggerExit(Collider collider)
        {
            if (!IsServer) return;
            if (collider.gameObject == PlayerTarget)
            {
                _enemy.SetStrikingDistanceBool(false);
            }
        }
    }
}
