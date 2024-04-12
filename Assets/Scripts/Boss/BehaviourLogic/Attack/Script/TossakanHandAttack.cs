using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.Netcode;

namespace Enemy
{
    [CreateAssetMenu(fileName = "HandAttack", menuName = "Boss/Boss Logic/Attack Pattern/Hand")]
    public class TossakanHandAttack : EnemyAttack
    {
        private TossakanBossController tossakan;
        [Header("Hand Attack Parameter")]
        [SerializeField] private int[] attackSequence;
        [SerializeField] private float timeBetweenAttackSet;
        [SerializeField] private float timeBetweenHands;
        [SerializeField] private GameObject[] handPrefab;

        private readonly int invincibleTrigger = Animator.StringToHash("Invinsible");
        private readonly int vincibleTrigger = Animator.StringToHash("Vinsible");

        public override void Initialize(GameObject targetPlayer, GameObject enemyGameObject, DamageCalculationComponent component)
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
            tossakan.tossakanPuppet.animator.SetTrigger(invincibleTrigger);
            tossakan.StartCoroutine(HandAttackSequence());
        }

        private IEnumerator HandAttackSequence()
        {
            var hand = SelectHand();

            for (int i = 0; i < attackSequence.Length; i++)
            {
                Transform[] spawnLocation = RandomSpawnPosition(attackSequence[i]);
                for (int j = 0; j < spawnLocation.Length; j++)
                {
                    var handInstance = Instantiate(hand, spawnLocation[j].position, spawnLocation[j].rotation);
                    handInstance.transform.LookAt(enemy.targetPlayer.transform);

                    if (handInstance.TryGetComponent<TossakanArmBased>(out var arm))
                        arm.Initialize(enemy.dealerPipeline);

                    if (handInstance.TryGetComponent<TossakanHitboxDamageable>(out var damageable))
                        damageable.Initialize(enemy, enemy.dealerPipeline);

                    if (handInstance.TryGetComponent<NetworkObject>(out var networkObject))
                        networkObject.Spawn();

                    yield return new WaitForSeconds(timeBetweenHands);
                }
                yield return new WaitForSeconds(timeBetweenAttackSet);
            }

            tossakan.tossakanPuppet.animator.SetTrigger(vincibleTrigger);
            EmitAttackEndsEvent();
        }

        private Transform[] RandomSpawnPosition(int numbers)
        {
            System.Random rnd = new System.Random();
            Transform[] spawnGroup = tossakan.handSpawnSet.Cast<Transform>().ToArray();

            // if number < spawn group length
            //  select random spawn group
            //  for each of that, select one random
            //
            // if number >= spawn group length
            //  random sort group
            //  for each of spawn group
            //   calculate how many then select

            List<Transform> listOfSelectedSpawnPoint = new List<Transform>();

            if (numbers < spawnGroup.Length)
            {
                var selectedSpawnGroups = spawnGroup.OrderBy(x => rnd.Next()).Take(numbers).ToList();
                foreach (Transform t in selectedSpawnGroups)
                {
                    Transform[] spawnPoint = t.Cast<Transform>().ToArray();
                    if (spawnPoint.Length == 0) continue;
                    int rndIndex = rnd.Next(0, spawnPoint.Length);
                    listOfSelectedSpawnPoint.Add(spawnPoint[rndIndex]);
                }

                return listOfSelectedSpawnPoint.ToArray();
            }

            int remainder = numbers % spawnGroup.Length;
            var shuffledSpawnGroup = spawnGroup.OrderBy(x => rnd.Next()).ToArray();

            for (int i = 0; i < shuffledSpawnGroup.Length; i++)
            {
                int numbersFromThisGroup = (numbers / spawnGroup.Length) + (i < remainder ? 1 : 0);
                var selectedSpawnPoints = shuffledSpawnGroup[i].Cast<Transform>().OrderBy(x => rnd.Next()).Take(numbers);
                listOfSelectedSpawnPoint.AddRange(selectedSpawnPoints);
            }

            return listOfSelectedSpawnPoint.ToArray();
        }

        private GameObject SelectHand()
        {
            System.Random rnd = new System.Random();
            if (handPrefab.Length == 0) return null;
            return handPrefab[rnd.Next(0, handPrefab.Length)];
        }
    }
}
