using UnityEngine;
using System.Collections;
using System.Linq;

namespace Enemy
{
    [CreateAssetMenu(fileName = "Chase Running", menuName = "Enemy/Enemy State/Chase State/Chase Running")]
    public class EnemyChaseRunning : EnemyChaseSOBase
    {
        [SerializeField] private float chaseSpeed = 5.0f;
        [SerializeField] private float targetCheckInterval = 1f;
        private EnemyWithinTriggerCheck strikingDistanceCheck;
        private bool isMoving = false;

        [Header("Chase Audio")]
        [SerializeField] private string[] listOfChaseSoundName;
        [SerializeField] private float timeBetweenSteps;

        public override void Initialize(GameObject gameObject, EnemyBase enemy)
        {
            base.Initialize(gameObject, enemy);
            strikingDistanceCheck = enemy.transform.Find("StrikingDistance")?.GetComponent<EnemyWithinTriggerCheck>();
            if (strikingDistanceCheck == null) Debug.LogError("Enemy has no Striking Distance Check");
        }

        public override void DoEnterLogic()
        {
            base.DoEnterLogic();
            if (!enemy.IsServer) return;
            enemy.animator.SetTrigger(enemy.startChasingAnimationTrigger);

            isMoving = true;
            enemy.navMeshAgent.speed = chaseSpeed;
            enemy.StartCoroutine(Move());
            enemy.StartCoroutine(GenerateChaseSound());
        }

        public override void DoExitLogic()
        {
            base.DoExitLogic();
            isMoving = false;
        }

        public override void DoFrameUpdateLogic()
        {
            base.DoFrameUpdateLogic();

            // transform.LookAt(enemy.targetPlayer.transform);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

            if (strikingDistanceCheck.PlayerWithinTrigger.Count != 0)
            {
                enemy.StateMachine.ChangeState(enemy.AttackState);
            }
        }

        public IEnumerator Move()
        {
            while (isMoving)
            {
                enemy.navMeshAgent?.SetDestination(enemy.targetPlayer.transform.position);
                yield return new WaitForSeconds(targetCheckInterval);
            }
        }

        public IEnumerator GenerateChaseSound()
        {
            for (int i = 0; i < listOfChaseSoundName.Length; i++)
            {
                if (!enemy.audioController.CheckIsSoundAvailable(listOfChaseSoundName[i]))
                {
                    Debug.LogError("Check Sound Name in Chase Running State: " + enemy);
                    yield break;
                }
            }

            System.Random rnd = new System.Random();
            var chosenSound = listOfChaseSoundName.OrderBy(x => rnd.Next()).Take(2).ToList();

            int counter = 0;
            while (isMoving)
            {
                var sound = chosenSound[counter];
                enemy.audioController.PlaySFXAtObject(sound, enemy.transform.position);
                if (++counter > 1) counter = 0;
                yield return new WaitForSeconds(timeBetweenSteps);
            }
        }
    }
}
