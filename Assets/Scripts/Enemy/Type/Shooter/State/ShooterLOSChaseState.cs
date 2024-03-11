using System.Collections;
using UnityEngine;

namespace Enemy
{
    [CreateAssetMenu(fileName = "LineOfSightCheckChase", menuName = "Enemy/Enemy State/Chase State/Line Of Sight Check")]
    public class ShooterLOSChaseState : EnemyChaseSOBase
    {
        [SerializeField] float checkCooldownTime = 1.0f;
        EnemyLineOfSightCheck lineOfSightCheck;

        public override void Initialize(GameObject gameObject, EnemyBase enemy)
        {
            base.Initialize(gameObject, enemy);
            lineOfSightCheck = enemy.gameObject.GetComponentInChildren<EnemyLineOfSightCheck>();
            if (lineOfSightCheck == null) Debug.LogError("Line of sight check not found");
        }

        public override void DoEnterLogic()
        {
            base.DoEnterLogic();
            enemy.StartCoroutine(CheckLineOfSight());
        }

        public override void DoExitLogic()
        {
            base.DoExitLogic();
            enemy.StopCoroutine(CheckLineOfSight());
        }

        public override void DoFrameUpdateLogic()
        {
            base.DoFrameUpdateLogic();
            enemy.transform.LookAt(enemy.targetPlayer.transform);
            enemy.transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        }

        private IEnumerator CheckLineOfSight()
        {
            bool isStillChecking = true;

            while (isStillChecking)
            {
                if (lineOfSightCheck.IsPlayerInLineOfSight(enemy.targetPlayer))
                {
                    Debug.Log("Shooter Checking LOS: Found");
                    isStillChecking = false;
                    enemy.animator.SetTrigger(enemy.attackAnimationTrigger);
                    enemy.StateMachine.ChangeState(enemy.AttackState);
                }
                else
                {
                    Debug.Log("Shooter Checking LOS: Not Found");
                    yield return new WaitForSeconds(checkCooldownTime);
                }
            }
        }
    }
}
