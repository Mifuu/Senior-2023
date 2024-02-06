using UnityEngine;
using ObserverPattern;

namespace Enemy
{
    [CreateAssetMenu(menuName = "Enemy/Enemy Damage Unit/Base Attack", fileName = "Base Attack")]
    public class DCUBaseAtk : DamageCalculationUnit<float>
    {
        // Static Unit 
        Subject<float> BaseAtk;

        public override void Setup()
        {
            BaseAtk = AddParameterToTrackList("BaseAtk", gameObject.GetComponent<EnemyBase>().BaseAtk);
            if (BaseAtk == null)
            {
                Debug.LogError(gameObject + " Base Damage Calculation Setup Fault");
                IsEnabled = false;
            }
        }

        public override float PreCalculate(float initialValue)
        {
            return BaseAtk.Value;
        }
    }
}
