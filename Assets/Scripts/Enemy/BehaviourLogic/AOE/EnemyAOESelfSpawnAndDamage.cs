using UnityEngine;
using System.Collections;
using Unity.Netcode;

namespace Enemy
{
    public class EnemyAOESelfSpawnAndDamage : EnemyAOEBase
    {
        [SerializeField] private float timeTillDestroy = 3.0f;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (!IsServer) return;
            transform.position = enemy.transform.position;
            StartCoroutine(DestroyAOE());
        }

        public void OnTriggerEnter(Collider collider)
        {
            var info = component.GetFinalDealthDamageInfo();
            info.dealer = enemy.gameObject;
            var damager = collider.GetComponentInChildren<IDamageCalculatable>();
            if (damager == null) return;
            damager.Damage(info);
        }

        private IEnumerator DestroyAOE()
        {
            yield return new WaitForSeconds(timeTillDestroy);
            if (!isActiveAndEnabled) yield break;
            if (TryGetComponent<NetworkObject>(out var networkObj))
                networkObj.Despawn(true);
        }
    }
}
