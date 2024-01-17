using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace Enemy
{
    public class EnemyBullet : NetworkBehaviour, IDamageable, IDamageCalculatable
    {
        [SerializeField] private float bulletSpeed = 100f;
        [SerializeField] private GameObject bulletOwner;
        [SerializeField] private bool isHomingCapable;
        [SerializeField] private float baseDamageAmount = 5.0f;
        private GameObject target;

        #region Damageable

        public float maxHealth { get; set; }
        public NetworkVariable<float> currentHealth { get; set; }

        #endregion

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            ResetValue();
        }

        public void Shoot(GameObject bulletOwner, GameObject target)
        {
            this.target = target;
            this.bulletOwner = bulletOwner;
        }

        public void Update()
        {
            if (!isHomingCapable) return;
            gameObject.transform.LookAt(target.transform);
            transform.Translate(Vector3.forward * (bulletSpeed * Time.deltaTime));
        }

        private DamageInfo Damage(IDamageCalculatable damagable)
        {
            DamageInfo info = new DamageInfo();
            if (damagable == null) return info;

            info.dealer = gameObject;
            info.amount = baseDamageAmount;
            damagable.Damage(info);

            return info;
        }

        public void OnTriggerEnter(Collider collider)
        {
            var damager = collider.GetComponent<IDamageCalculatable>();
            Damage(damager);
        }

        public void Damage(float damageAmount)
        {
            currentHealth.Value -= damageAmount;
            if (currentHealth.Value <= 0)
            {
                Die();
            }
        }

        public void Die()
        {
            throw new System.NotImplementedException();
        }

        public void Damage(DamageInfo damageInfo)
        {
            Damage(damageInfo.amount);
        }

        public float getCurrentHealth()
        {
            return currentHealth.Value;
        }

        private void ResetValue()
        {
            this.bulletOwner = null;
        }
    }
}

