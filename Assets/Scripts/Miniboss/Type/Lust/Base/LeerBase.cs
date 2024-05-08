using UnityEngine;
using Unity.Netcode;
using System.Collections;
using System;

namespace Enemy
{
    [RequireComponent(typeof(VisibleObjectCheckable))]
    public class LeerBase : NetworkBehaviour
    {
        [Header("Damage Config")]
        [Tooltip("Eye attack uses base damage from the boss itself then multiply with the factor")]
        [SerializeField] private float damageFactor = 0.1f;
        [Tooltip("Damage Interval in seconds")]
        [SerializeField] private float damageInterval = 0.5f;

        [Header("Timing")]
        [Tooltip("Time from the eye changing from the preperation phase to attack phase")]
        [SerializeField] private float timeUntilAttack = 1.0f;
        [Tooltip("Time from the eye being visible to the damage begining")]
        [SerializeField] private float timeUntilTakeDamage = 0.5f;
        [Tooltip("Time from the Attach phase to the end")]
        [SerializeField] private float timeUntilAttackEnds = 8.0f;

        private VisibleObjectCheckable visibleObject;
        private EnemyBase enemy;
        private NetworkObject clientPlayerObject;
        private NetworkObject thisNetworkObject;
        private IDamageCalculatable playerDamageable;
        private DamageInfo damageInfo;
        private IEnumerator damageCoroutine;

        public event Action OnLeerAttackEnds;

        #region
        private Animator animator;
        #endregion

        public void Awake()
        {
            animator = GetComponent<Animator>();
            visibleObject = GetComponent<VisibleObjectCheckable>();
            damageCoroutine = OnLeerBecomeVisible();
        }

        /* public void Update() */
        /* { */
        /*     transform.LookAt(enemy.targetPlayer.transform); */
        /* } */

        public void InitializeLeer(EnemyBase enemy)
        {
            this.enemy = enemy;
            if (enemy == null)
            {
                Debug.LogError("Enemy is not found");
            }
            damageInfo = enemy.GetComponent<DamageCalculationComponent>().GetFinalDealthDamageInfo();
            damageInfo.amount = damageInfo.amount * damageFactor;
        }

        public override void OnNetworkSpawn()
        {
            clientPlayerObject = NetworkManager.Singleton.LocalClient.PlayerObject;
            playerDamageable = clientPlayerObject.GetComponentInChildren<IDamageCalculatable>();
            thisNetworkObject = GetComponent<NetworkObject>();
            StartCoroutine(AttackSequence());
        }

        private IEnumerator AttackSequence()
        {
            // Begin Prep phase
            yield return new WaitForSeconds(timeUntilAttack);
            // Begin Attack phase
            visibleObject.isVisible.OnValueChanged += VisibilityChecker;
            yield return new WaitForSeconds(timeUntilAttackEnds);
            visibleObject.isVisible.OnValueChanged -= VisibilityChecker;
            StopCoroutine(damageCoroutine);
            OnLeerAttackEnds?.Invoke();
            thisNetworkObject.Despawn();
        }

        private void VisibilityChecker(bool prev, bool current)
        {
            if (current)
                StartCoroutine(damageCoroutine);
            else
                StopCoroutine(damageCoroutine);
        }

        private IEnumerator OnLeerBecomeVisible()
        {
            while (true)
            {
                playerDamageable.Damage(damageInfo);
                yield return new WaitForSeconds(damageInterval);
            }
        }
    }
}
