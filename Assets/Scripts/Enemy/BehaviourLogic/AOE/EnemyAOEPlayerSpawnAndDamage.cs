using UnityEngine;

namespace Enemy
{
    public class EnemyAOEPlayerSpawnAndDamage : EnemyAOEPlayerSpawnAndActivate
    {
        public override void ActivateEffect()
        {
            base.ActivateEffect();
            foreach (var players in areaOfEffectTrigger.PlayerWithinTrigger)
            {
                var info = enemy.dealerPipeline.GetFinalDealthDamageInfo();
                info.dealer = enemy.gameObject;

                var damager = players.GetComponentInChildren<IDamageCalculatable>();
                if (damager == null)
                {
                    Debug.LogError("IDamageCalculatable Not found On Object " + players);
                    return;
                }

                damager.Damage(info);
            }
        }

        public override void CancelEffect()
        {
            base.CancelEffect();
            // Debug.Log("Canceling Effect");
            EmitAOEEndsEvent();
            throw new System.NotImplementedException("Implement EnemyAOEPlayerSpawnAndDamage CancelEffect Function");
        }
    }
}
