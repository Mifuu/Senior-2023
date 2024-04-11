using System.Collections;
using UnityEngine;
using System.Linq;

namespace Enemy
{
    [CreateAssetMenu(fileName = "LineOfSightCheckChase", menuName = "Enemy/Enemy State/Chase State/Line Of Sight Check")]
    public class ShooterLOSChaseState : EnemyChaseSOBase
    {
        [SerializeField] float checkCooldownTime = 1.0f;
        [SerializeField] float chaseSpeed = 6.0f;
        EnemyLineOfSightCheck lineOfSightCheck;

        [Header("Chase Audio")]
        [SerializeField] private string[] listOfChaseSoundName;
        [SerializeField] private float timeBetweenSteps;

        IEnumerator checkLineOfSightCoroutine;
        IEnumerator generateFootStepCoroutine;
        private bool isMoving;

        public override void Initialize(GameObject gameObject, EnemyBase enemy)
        {
            base.Initialize(gameObject, enemy);
            lineOfSightCheck = enemy.gameObject.GetComponentInChildren<EnemyLineOfSightCheck>();
            if (lineOfSightCheck == null) Debug.LogError("Line of sight check not found");
            enemy.navMeshAgent.speed = chaseSpeed;
            isMoving = false;
        }

        public override void DoEnterLogic()
        {
            base.DoEnterLogic();
            isMoving = false;
            checkLineOfSightCoroutine = CheckLineOfSight();
            generateFootStepCoroutine = GenerateChaseSound();
            enemy.StartCoroutine(checkLineOfSightCoroutine);
            enemy.StartCoroutine(generateFootStepCoroutine);
        }

        public override void DoExitLogic()
        {
            base.DoExitLogic();
            isMoving = false;
            enemy.StopCoroutine(checkLineOfSightCoroutine);
            enemy.StopCoroutine(generateFootStepCoroutine);
        }

        private IEnumerator CheckLineOfSight()
        {
            bool isStillChecking = true;

            while (isStillChecking)
            {
                if (lineOfSightCheck.IsTargetPlayerInLineOfSight())
                {
                    isStillChecking = false;
                    isMoving = false;
                    enemy.navMeshAgent.SetDestination(enemy.transform.position);
                    enemy.StateMachine.ChangeState(enemy.AttackState);
                }
                else
                {
                    isMoving = true;
                    enemy.navMeshAgent.SetDestination(enemy.targetPlayer.transform.position);
                    yield return new WaitForSeconds(checkCooldownTime);
                }
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
