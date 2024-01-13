using UnityEngine;

namespace Enemy
{
    [CreateAssetMenu(fileName = "Chase Running", menuName = "Enemy/Enemy State/Chase State/Chase Running")]
    public class EnemyChaseRunning : EnemyChaseSOBase
    {
        [SerializeField] private float chaseSpeed = 5.0f;
        private ITriggerCheckable strikingDistanceCheck;

        public override void Initialize(GameObject gameObject, EnemyBase enemy)
        {
            base.Initialize(gameObject, enemy);
            strikingDistanceCheck = enemy.transform.Find("StrikingDistance")?.GetComponent<ITriggerCheckable>();
            if (strikingDistanceCheck == null) Debug.LogError("Enemy has no Striking Distance Check");
        }

        public override void DoFrameUpdateLogic()
        {
            base.DoFrameUpdateLogic();
            transform.LookAt(playerTransform);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
            transform.Translate(Vector3.forward * (chaseSpeed * Time.deltaTime));

            Debug.Log("Current #Object in Strikezone: " + strikingDistanceCheck.ObjectsInTrigger.Count);
            Debug.Log("Current #Players in Strikezone: " + strikingDistanceCheck.PlayerWithinTrigger.Count);

            // TODO: Test the method to check striking distance
            if (strikingDistanceCheck.PlayerWithinTrigger.Count != 0)
            {
                enemy.StateMachine.ChangeState(enemy.AttackState);
            }
        }
    }
}
