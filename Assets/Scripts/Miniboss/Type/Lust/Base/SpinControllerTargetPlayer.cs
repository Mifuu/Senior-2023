using UnityEngine;
using Unity.Netcode;

namespace Enemy
{
    public class SpinControllerTargetPlayer : NetworkBehaviour
    {
        [SerializeField] EnemyBase enemy;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (!IsServer) enabled = false;
        }

        public void FixedUpdate()
        {
            transform.LookAt(enemy.targetPlayer.transform);
        }
    }
}
