namespace Enemy
{
    public class TossakanHitboxDamageable : EnemyHitboxDamageable
    {
        public override void Start() {}

        public void Initialize(EnemyBase enemy, DamageCalculationComponent damageComponent)
        {
            this.enemy = enemy;
            this.damageComponent = damageComponent;
        }
    }
}
