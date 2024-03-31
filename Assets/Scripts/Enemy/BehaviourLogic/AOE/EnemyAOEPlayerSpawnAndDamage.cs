using UnityEngine;

namespace Enemy
{
    public class EnemyAOEPlayerSpawnAndDamage : EnemyAOEPlayerSpawnAndActivate
    {
        [SerializeField] public float pushBackForce = 10.0f;

        public override void ActivateEffect()
        {
            base.ActivateEffect();
            // BUG: DAMAGE CALCULATION COMPONENT IS DESTROYED WTFFFFFFFFFFFFF
            foreach (var players in areaOfEffectTrigger.PlayerWithinTrigger)
            {
                var info = component.GetFinalDealthDamageInfo();
                info.dealer = enemy.gameObject;

                // Push back
                // NullReference, Player do not have RigidBody
                // var rb = players.GetComponent<Rigidbody>();
                // rb.AddForce(GenerateRandomDirection(), ForceMode.Impulse);

                // Do damage
                var damager = players.GetComponentInChildren<IDamageCalculatable>();
                if (damager == null)
                {
                    Debug.LogError("IDamageCalculatable Not found On Object " + players);
                    return;
                }

                damager.Damage(info);
            }
        }

        public Vector3 GenerateRandomDirection()
        {
            float x = Random.Range(-1f, 1f);
            float z = Random.Range(-1f, 1f);
            return new Vector3(x, 0.5f, z).normalized * pushBackForce;
        }

        public Vector3 GenerateDirectedPushbackDirection()
        {
            // TODO: Define the proper pushback direction
            return Vector3.zero;
        }

        public override void CancelEffect()
        {
            base.CancelEffect();
            EmitAOEEndsEvent();
            throw new System.NotImplementedException("Implement EnemyAOEPlayerSpawnAndDamage CancelEffect Function");
        }
    }
}
