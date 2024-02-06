using UnityEngine;
using ObserverPattern;

namespace Enemy
{
    [CreateAssetMenu(menuName = "Enemy/Enemy Damage Unit/Elemental Damage", fileName = "Elemental Damage Calculator")]
    public class DCUElementalDamage : DamageCalculationUnit<float>
    {
        // Static Unit
        EnemyBase enemy;
        Subject<float> hydro;
        Subject<float> pyro;
        Subject<float> electro;

        public override void Setup()
        {
            enemy = gameObject.GetComponent<EnemyBase>();
            hydro = AddParameterToTrackList("hydro", enemy.HydroDamageBonus);
            pyro = AddParameterToTrackList("pyro", enemy.PyroDamageBonus);
            electro = AddParameterToTrackList("electro", enemy.ElectroDamageBonus);

            if (hydro == null || pyro == null || electro == null)
            {
                Debug.LogError("Elemental Damage Calculation Setup Fault");
                IsEnabled = false;
            }
        }

        public override float PreCalculate(float initialValue)
        {
            // Not the real function, just testing
            return initialValue * hydro.Value * pyro.Value * electro.Value;
        }
    }
}
