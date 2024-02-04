using UnityEngine;

namespace Enemy
{
    [CreateAssetMenu(menuName = "Enemy/Enemy Damage Unit/Base Attack")]
    public class DCUBaseAtk : DamageCalculationUnit<float>
    {
        public override void Initialize(IDamageCalculationPipelineBase pipelineBase, bool updateOnChange, GameObject owner)
        {
            base.Initialize(pipelineBase, updateOnChange, owner);
            AddParameter("BaseAtk", gameObject.GetComponent<EnemyBase>().BaseAtk);
        }

        public override float PreCalculate(float initialValue)
        {
            return TryGetParameter("BaseAtk");
        }
    }
}
