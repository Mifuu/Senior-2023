using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    [CreateAssetMenu(fileName = "LineOfSightCheckChase", menuName = "Enemy/Enemy State/Chase State/Line Of Sight Check")]
    public class ShooterLOSChaseState : EnemyChaseSOBase
    {
        [SerializeField] float checkCooldownTime = 5.0f;
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
            enemy.PerformCoroutine(CheckLineOfSight());
        }

        public override void DoExitLogic()
        {
            base.DoExitLogic();
            enemy.PerformStopCoroutine(CheckLineOfSight());
        }

        public override void DoFrameUpdateLogic()
        {
            base.DoFrameUpdateLogic();
            enemy.transform.LookAt(playerTransform);
            enemy.transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        }

        private IEnumerator CheckLineOfSight()
        {
            bool isStillChecking = true;

            while (isStillChecking)
            {
                Debug.Log("Shooter checking LOS, State: Chase");
                if (lineOfSightCheck.IsPlayerInLineOfSight())
                {
                    isStillChecking = false;
                    enemy.StateMachine.ChangeState(enemy.AttackState);
                }
                else
                {
                    yield return new WaitForSeconds(checkCooldownTime);
                }
            }
        }
    }
}
