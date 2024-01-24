using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    [CreateAssetMenu(fileName = "Shoot Attack", menuName = "Enemy/Enemy Logic/Attack Pattern/Shoot")]
    public class ShootAttack : EnemyAttack
    {
        [Header("Shoot Attack Attribute")]
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private int bulletAmount = 1;
        [SerializeField] private float preShootDelay = 1.0f;
        [SerializeField] private float bulletDelay = 0.1f;
        private Rigidbody bulletRB;
        private GameObject bulletSpawn;

        public override void Initialize(GameObject targetPlayer, GameObject enemyGameObject)
        {
            base.Initialize(targetPlayer, enemyGameObject);
            bulletRB = bulletPrefab.GetComponent<Rigidbody>();
            bulletSpawn = enemy.transform.Find("BulletSpawn")?.gameObject;
            if (bulletSpawn == null)
                Debug.LogError("Bullet spawn is not found");
        }

        public override void PerformAttack()
        {
            if (!enemy.IsServer) return;
            enemy.StartCoroutine(ShootAttackCoroutine());
        }

        private IEnumerator ShootAttackCoroutine()
        {
            yield return new WaitForSeconds(preShootDelay);
            for (int i = 0; i < bulletAmount; i++)
            {
                var newBullet = NetworkObjectPool.Singleton.GetNetworkObject(bulletPrefab, bulletSpawn.transform.position, enemy.transform.rotation);
                enemy.transform.LookAt(enemy.targetPlayer.transform);
                newBullet.Spawn();
                newBullet.gameObject.GetComponent<EnemyBullet>().InitializeAndShoot(enemy.gameObject, enemy.targetPlayer);
                yield return new WaitForSeconds(bulletDelay);
            }
            EmitAttackEndsEvent();
        }
    }
}
