using System.Collections;
using UnityEngine;
using System.Linq;
using UnityEngine.VFX;
using Unity.Netcode;

namespace Enemy
{
    [CreateAssetMenu(fileName = "TossakanChase", menuName = "Enemy/Enemy State/Chase State/Tossakan Chase")]
    public class TossakanChaseState : EnemyChaseSOBase
    {
        [SerializeField] private float[] phaseTeleportOdds;
        [SerializeField] private float idleCooldownTime;
        [SerializeField] private float waitForTeleportTime; // Time from Tossakan Disappear to Change Position
        [SerializeField] private float postTeleportCooldown;
        [SerializeField] private VisualEffect spawnFlame;

        private System.Random rnd = new System.Random();
        private TossakanBossController tossakan;
        private Transform[] allAnchor;
        private bool isTeleporting = false;

        private readonly int teleportAnimTrigger = Animator.StringToHash("Teleport");

        public override void Initialize(GameObject gameObject, EnemyBase enemy)
        {
            base.Initialize(gameObject, enemy);
            if (!(enemy is TossakanBossController))
            {
                Debug.LogError("Only Tossakan is allowed to use this attack");
                return;
            }

            if (phaseTeleportOdds.Length != 2)
            {
                Debug.LogError("Phase Teleport odds length is not 2");
                return;
            }

            tossakan = enemy as TossakanBossController;
            allAnchor = tossakan.allTossakanPositionAnchor.transform.Cast<Transform>().ToArray();
        }

        public override void DoEnterLogic()
        {
            base.DoEnterLogic();
            enemy.StartCoroutine(TeleportSequence());
        }

        private IEnumerator TeleportSequence()
        {
            yield return new WaitForSeconds(idleCooldownTime);
            if (RandomWillTeleport())
                enemy.StartCoroutine(RandomPositionAndTeleport());
            else
                enemy.StateMachine.ChangeState(enemy.AttackState);
        }

        private bool RandomWillTeleport() => (rnd.NextDouble() <= phaseTeleportOdds[tossakan.currentPhase.Value - 1]);

        private IEnumerator RandomPositionAndTeleport()
        {
            Transform spawnInfo = allAnchor[rnd.Next(0, allAnchor.Length)];

            tossakan.allTossakanPositionAnchor.GetComponent<TossakanAnchors>().ResetAnchorRotation();
            tossakan.tossakanPuppet.animationEventEmitter.OnChaseAnimationEnds += OnTeleportAttackEnds;
            // SpawnBlackFlameAtLocation(enemy.transform.position);
            tossakan.tossakanPuppet.animator.SetTrigger(teleportAnimTrigger);
            yield return new WaitForSeconds(waitForTeleportTime);
            // SpawnBlackFlameAtLocation(spawnInfo.position);
            tossakan.tossakanPuppet.transform.localPosition = Vector3.zero;
            tossakan.tossakanPuppet.transform.position = spawnInfo.position;
            tossakan.tossakanPuppet.transform.rotation = spawnInfo.rotation;
        }

        private void SpawnBlackFlameAtLocation(Vector3 location)
        {
            var flameInstance = Instantiate(spawnFlame, location, Quaternion.identity);
            if (flameInstance.TryGetComponent<NetworkObject>(out var networkObject))
                networkObject.Spawn();
        }

        private void OnTeleportAttackEnds() => enemy.StartCoroutine(WaitAfterTeleport());

        private IEnumerator WaitAfterTeleport()
        {
            tossakan.tossakanPuppet.animationEventEmitter.OnChaseAnimationEnds -= OnTeleportAttackEnds;
            yield return new WaitForSeconds(postTeleportCooldown);
            enemy.StateMachine.ChangeState(enemy.AttackState);
        }
    }
}
