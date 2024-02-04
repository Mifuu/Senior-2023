using UnityEngine;

namespace Enemy
{
    [CreateAssetMenu(menuName = "Enemy/Enemy Damage Unit/Elemental Damage")]
    public class DCUElementalDamage : DamageCalculationUnit<float>
    {
        EnemyBase enemy;

        public override void Initialize(IDamageCalculationPipelineBase pipelineBase, bool updateOnChange, GameObject gameObject)
        {
            base.Initialize(pipelineBase, updateOnChange, gameObject);
            enemy = gameObject.GetComponent<EnemyBase>();
            AddParameter("HydroDamageBonus", enemy.HydroDamageBonus);
            AddParameter("PyroDamageBonus", enemy.PyroDamageBonus);
            AddParameter("ElectroDamageBonus", enemy.ElectroDamageBonus);
        }

        public override float PreCalculate(float initialValue)
        {
            var hydro = TryGetParameter("HydroDamageBonus");
            var pyro = TryGetParameter("PyroDamageBonus");
            var electro = TryGetParameter("ElectroDamageBonus");
            return initialValue * hydro * pyro * electro;
        }
    }
}
