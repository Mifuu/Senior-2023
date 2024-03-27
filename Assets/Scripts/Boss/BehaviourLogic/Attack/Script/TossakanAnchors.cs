using UnityEngine;
using Unity.Netcode;

namespace Enemy
{
    public class TossakanAnchors : NetworkBehaviour
    {
        [SerializeField] private EnemyBase enemy;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            ResetAnchorRotation();
        }

        public void ResetAnchorRotation()
        {
            foreach (Transform t in transform)
                t.LookAt(enemy.targetPlayer.transform);
        }
    }
}
