using UnityEngine;

namespace Enemy
{
    [CreateAssetMenu(fileName = "AOEAttack", menuName = "Enemy/Enemy Logic/Attack Pattern/AOE")]
    public class AOEAttack : EnemyAttack
    {
        [Header("AOE Attack Attribute")]
        [SerializeField] private GameObject AOEGameObject;
        [SerializeField] private bool waitForAOEToEnd = false;
        [SerializeField] private float maxSqrDistance = 1000f;

        private EnemyAOEBase aoeBase;

        public override void PerformAttack()
        {
            base.PerformAttack();
            if (!enemy.IsServer) return;

            if ((enemy.transform.position - enemy.targetPlayer.transform.position).sqrMagnitude > maxSqrDistance)
            {
                EmitAttackEndsEvent();
                return;
            }

            var aoe = NetworkObjectPool.Singleton.GetNetworkObject(AOEGameObject, enemy.transform.position, AOEGameObject.transform.rotation);
            aoeBase = aoe.gameObject.GetComponent<EnemyAOEBase>();

            if (aoeBase == null)
            {
                Debug.LogError("No AOE Base object, exiting");
                return;
            }
            aoeBase.InitializeAOE(enemy.targetPlayer, enemy);
            aoe.Spawn();

            if (waitForAOEToEnd)
            {
                aoeBase.OnAOEPeriodEnd += FinishingAttackCallback;
            }
            else
                EmitAttackEndsEvent();
        }

        public void FinishingAttackCallback()
        {
            EmitAttackEndsEvent();
            aoeBase.OnAOEPeriodEnd -= FinishingAttackCallback;
        }
    }
}
