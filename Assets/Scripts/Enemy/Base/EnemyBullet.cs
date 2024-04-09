using UnityEngine;
using Unity.Netcode;
using System.Collections;

namespace Enemy
{
    public class EnemyBullet : NetworkBehaviour, IDamageable
    {
        [SerializeField] private float bulletSpeed = 10f;
        [SerializeField] private bool isHomingCapable;
        [SerializeField] private float baseDamageAmount = 5.0f;

        private GameObject target;
        private GameObject bulletOwner;
        private DamageCalculationComponent component;
        private Rigidbody rb;
        private readonly float bulletLifeSpan = 10f;
        private IEnumerator bulletCountdownCoroutine;

        #region Damageable

        [field: SerializeField] public float maxHealth { get; set; }
        public NetworkVariable<float> currentHealth { get; set; } = new NetworkVariable<float>(-1.0f);

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
            bulletCountdownCoroutine = BulletDespawnTimedCountdown();
            StartCoroutine(bulletCountdownCoroutine);
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            ResetValue();
        }

        public void InitializeAndShoot(GameObject bulletOwner, GameObject target, DamageCalculationComponent component = null)
        {
            this.target = target;
            this.bulletOwner = bulletOwner;

            var targetPlayer = target.GetComponent<PlayerHealth>();
            targetPlayer.OnPlayerDie += OnTargetPlayerDie;

            if (component == null)
                this.component = bulletOwner.GetComponent<DamageCalculationComponent>();
            else
                this.component = component;
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

            info = component.GetFinalDealthDamageInfo();
            damagable.Damage(info);

            return info;
        }

        public void OnTriggerEnter(Collider collider)
        {
            if (!IsServer) return;

            if (collider.TryGetComponent<IDamageCalculatable>(out var damager)) 
                DamageDamageable(damager);

            Die(null);
        }

        public void Damage(float damageAmount, GameObject dealer)
        {
            if (!IsServer) return;
            currentHealth.Value -= damageAmount;
            if (currentHealth.Value <= 0)
                Die(dealer);
        }

        public void Die()
        {
            // Remove the Parameter When called with event
            Die(null);
        }

        public void Die(GameObject killer)
        {
            if (!IsServer) return;
            if (!isActiveAndEnabled) return;

            StopCoroutine(bulletCountdownCoroutine);

            if (this.target != null && this.target.TryGetComponent<PlayerHealth>(out var targetPlayer))
                targetPlayer.OnPlayerDie -= OnTargetPlayerDie;

            var networkObj = GetComponent<NetworkObject>();
            networkObj.Despawn();
        }

        private void OnTargetPlayerDie()
        {
            target.GetComponent<PlayerHealth>().OnPlayerDie -= OnTargetPlayerDie;
            isHomingCapable = false;
            target = null;
        }

        private IEnumerator BulletDespawnTimedCountdown()
        {
            yield return new WaitForSeconds(bulletLifeSpan);
            Die();
        }

        private void ResetValue()
        {
            this.bulletOwner = null;
            bulletCountdownCoroutine = null;
            target = null;
        }
    }
}
