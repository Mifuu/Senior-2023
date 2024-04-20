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
            enabled = !(!IsServer || enemy.targetPlayer == null);
            enemy.OnTargetPlayerChanged += ChangeSpinStateOnTargetPlayer;
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            enemy.OnTargetPlayerChanged -= ChangeSpinStateOnTargetPlayer;
        }

        public void Awake() => enabled = false;
        private void ChangeSpinStateOnTargetPlayer(GameObject targetPlayer) => enabled = targetPlayer != null;
        public void FixedUpdate() => transform.LookAt(enemy.targetPlayer.transform);
    }
}
