using UnityEngine;
using Unity.Netcode;

namespace Enemy
{
    [CreateAssetMenu(fileName = "ArrowAttack", menuName = "Boss/Boss Logic/Attack Pattern/Arrow")]
    public class TossakanArrowAttack : EnemyAttack
    {
        private TossakanBossController tossakan;
        [Header("Arrow Attack Parameter")]
        [SerializeField] private GameObject arrowPrefab;

        public override void Initialize(GameObject targetPlayer, GameObject enemyGameObject, DamageCalculationComponent component = null)
        {
            base.Initialize(targetPlayer, enemyGameObject, component);
            if (!(enemy is TossakanBossController))
            {
                Debug.LogError("Only Tossakan is allowed to use this attack");
                Debug.LogError("Current Enemy: " + enemy);
                Debug.LogError("Current Attack: " + this);
                return;
            }
            tossakan = enemy as TossakanBossController;
        }

        public override void PerformAttack()
        {
            base.PerformAttack();
            foreach (Transform t in tossakan.arrowSpawnSet)
            {
                var arrowInstance = Instantiate(arrowPrefab, t.position, t.rotation);
                if (arrowInstance.TryGetComponent<NetworkObject>(out var networkObject))
                {
                    networkObject.Spawn();
                }
            }

            EmitAttackEndsEvent();
        }
    }
}
