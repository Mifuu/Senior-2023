using System.Collections;
using UnityEngine;

namespace Enemy
{
    [CreateAssetMenu(fileName = "AOEAttack", menuName = "Enemy/Enemy Logic/Attack Pattern/Multiple AOE")]
    public class MultipleAOEAttack : EnemyAttack
    {
        [Header("Multiple AOE Attack Attribute")]
        [SerializeField] private GameObject AOEGameObject;
        [SerializeField] private float timeBetweenAoe = 1.0f;
        [SerializeField] private int numberOfTime = 3;

        public override void PerformAttack()
        {
            base.PerformAttack();
            if (!enemy.IsServer) return;
            enemy.StartCoroutine(SpawnMultipleCoroutine());
        }

        private IEnumerator SpawnMultipleCoroutine()
        {
            int count = 0;
            while (count < numberOfTime)
            {
                var aoe = NetworkObjectPool.Singleton.GetNetworkObject(AOEGameObject, enemy.transform.position, AOEGameObject.transform.rotation);
                var aoeBase = aoe.gameObject.GetComponent<EnemyAOEBase>();
                if (aoeBase == null)
                {
                    Debug.LogError("No AOE Base object, exiting");
                    yield return null;
                }

                aoeBase.InitializeAOE(enemy.targetPlayer, enemy);
                aoe.Spawn();
                count++;
                yield return new WaitForSeconds(timeBetweenAoe);
            }

            EmitAttackEndsEvent();
        }
    }
}
