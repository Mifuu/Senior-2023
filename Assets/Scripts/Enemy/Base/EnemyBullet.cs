using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Events;

namespace Enemy
{
    public class EnemyBullet : NetworkBehaviour, IDamageable
    {
        [SerializeField] private float bulletSpeed = 10f;
        [SerializeField] private bool isHomingCapable;
        [SerializeField] private float baseDamageAmount = 5.0f;
        private GameObject target;
        private GameObject bulletOwner;
        private Rigidbody rb;

        #region Damageable

        [field: SerializeField] public float maxHealth { get; set; }
        public NetworkVariable<float> currentHealth { get; set; } = new NetworkVariable<float>(-1.0f);
        public UnityEvent OnHealthChanged { get; set; }

        #endregion

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (!IsServer)
            {
                enabled = false;
                return;
            }
            currentHealth.Value = maxHealth;
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            ResetValue();
        }

        public void InitializeAndShoot(GameObject bulletOwner, GameObject target)
        {
            this.target = target;
            this.bulletOwner = bulletOwner;

            var targetPlayer = target.GetComponent<PlayerHealth>();
            targetPlayer.OnPlayerDie += Die;
        }

        public void FixedUpdate()
        {
            transform.Translate(Vector3.forward * (bulletSpeed * Time.fixedDeltaTime));
            if (!isHomingCapable) return;
            gameObject.transform.LookAt(target.transform);
        }

        private DamageInfo DamageDamageable(IDamageCalculatable damagable)
        {
            DamageInfo info = new DamageInfo();
            if (damagable == null)
            {
                Die();
                return info;
            }

            var pipe = bulletOwner.GetComponent<DamageDealerCalculationPipeline>();
            Debug.Log("Bullet Damage Amount: " + pipe.GetFinalDealthDamageAmount());

            info.dealer = bulletOwner;
            info.amount = baseDamageAmount;
            damagable.Damage(info);

            // Debug.Log("DAMAGE: dealing " + info.amount + " DMG");
            return info;
        }

        public void OnTriggerEnter(Collider collider)
        {
            if (!IsServer) return;
            var damager = collider.GetComponent<IDamageCalculatable>();
            DamageDamageable(damager);

            // TODO: Bullet maynot die immediately after hit, as it may just pass through
            Die();
        }

        public void Damage(float damageAmount)
        {
            if (!IsServer) return;
            currentHealth.Value -= damageAmount;
            if (currentHealth.Value <= 0)
            {
                Die();
            }
        }

        public void Die()
        {
            if (!IsServer) return;

            if (!isActiveAndEnabled) return;

            var targetPlayer = this.target.GetComponent<PlayerHealth>();
            targetPlayer.OnPlayerDie -= Die;

            var networkObj = GetComponent<NetworkObject>();
            networkObj.Despawn();
        }

        private void ResetValue()
        {
            this.bulletOwner = null;
        }
    }
}

