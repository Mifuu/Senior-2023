using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

namespace Enemy
{
    [RequireComponent(typeof(BoxCollider))]
    public class CountBasedShield : NetworkBehaviour, IDamageCalculatable
    {
        [Header("Shield Setup")]
        [SerializeField] private int maxHealthCount;
        [SerializeField] private List<GameObject> shieldDestroyableGameObject;
        [SerializeField] private BoxCollider shieldCollider;

        [Header("Target Setup")]
        private EnemyHitboxDamageable targetEnemyDamageable;
        [SerializeField] private float damageFactor = 0;

        [HideInInspector] public NetworkVariable<int> shieldHealthCount = new NetworkVariable<int>(0);
        [HideInInspector] public NetworkVariable<bool> shieldUp = new NetworkVariable<bool>(false);

        public void Awake()
        {
            targetEnemyDamageable = GetComponentInParent<EnemyHitboxDamageable>();
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (IsServer)
            {
                shieldHealthCount.Value = maxHealthCount;
                shieldUp.Value = true;
            }
            shieldHealthCount.OnValueChanged += CheckHealth;
            shieldUp.OnValueChanged += ChangeShieldState;
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            if (!IsServer) return;
            shieldHealthCount.OnValueChanged -= CheckHealth;
            shieldUp.OnValueChanged -= ChangeShieldState;
        }

        public void DamageShield(int count)
        {
            if (!IsServer) return;
            shieldHealthCount.Value -= count;
        }

        public void ResetShieldState()
        {
            if (!IsServer) return;
            shieldUp.Value = true;
        }

        private void CheckHealth(int prev, int current)
        {
            if (!IsServer) return;
            shieldUp.Value = (current > 0);
        }

        private void ChangeShieldState(bool _, bool isEnabled)
        {
            shieldCollider.enabled = isEnabled;
            foreach (var toBeDestroyed in shieldDestroyableGameObject)
            {
                toBeDestroyed.SetActive(isEnabled);
            }
        }

        public virtual void Damage(DamageInfo damageInfo) { }

        public virtual float getCurrentHealth() => shieldHealthCount.Value;
    }
}
