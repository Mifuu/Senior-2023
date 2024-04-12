using UnityEngine;

namespace Enemy
{
    [CreateAssetMenu(fileName = "LeerAttack", menuName = "Enemy/Enemy Logic/Attack Pattern/Leer")]
    public class LeerAttack : EnemyAttack
    {
        [SerializeField] private GameObject eyeGameObject;
        [Tooltip("Height difference above the enemy to spawn")]
        [SerializeField] private float spawnHeightOffset;
        [Tooltip("Whether to wait for the Leer attack to ends to declare the ends of the attack")]
        [SerializeField] private bool waitForLeerToEnd;
        
        private LeerBase leerBase;

        public override void PerformAttack()
        {
            base.PerformAttack();
            if (!enemy.IsServer) return;    
            var eye = NetworkObjectPool.Singleton
                .GetNetworkObject(eyeGameObject, GetSpawnPosition(), eyeGameObject.transform.rotation);
            leerBase = eye.GetComponent<LeerBase>();
            if (leerBase == null)
            {
                Debug.LogError("No Leer Base Object, exiting");
                return;
            }

            leerBase.InitializeLeer(enemy);
            eye.Spawn();
            
            if (waitForLeerToEnd)
            {
                leerBase.OnLeerAttackEnds += FinishingAttackCallback; 
            }
            else 
            {
                EmitAttackEndsEvent();
            }
        }

        public Vector3 GetSpawnPosition()
        {
            var currentEnemyPosition = enemy.transform.position;
            return new Vector3(currentEnemyPosition.x, currentEnemyPosition.y + spawnHeightOffset, currentEnemyPosition.z);
        }

        private void FinishingAttackCallback()
        {
            EmitAttackEndsEvent();
            leerBase.OnLeerAttackEnds -= FinishingAttackCallback;
        }
    }
}
