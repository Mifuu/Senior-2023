using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    [CreateAssetMenu(fileName = "AOEAttack", menuName = "Enemy/Enemy Logic/Attack Pattern/AOE")]
    public class AOEAttack : EnemyAttack
    {
        [SerializeField] private GameObject AOEGameObject;
        [SerializeField] private bool waitForAOEToEnd = false;

        public override void PerformAttack()
        {
            // CONTINUE: Make the AOE Spawn on the ground
            var aoe = NetworkObjectPool.Singleton.GetNetworkObject(AOEGameObject, enemy.transform.position, AOEGameObject.transform.rotation);
            var aoeBase = aoe.gameObject.GetComponent<EnemyAOEBase>();
            if (aoeBase == null)
            {
                Debug.LogError("No AOE Base object, exiting");
                return;
            }
            aoeBase.ActivateAOE(targetPlayer, enemy);
            aoe.Spawn();
        }
    }
}
