using UnityEngine;
using System.Collections.Generic;

namespace Enemy
{
    public abstract class OrchestrationAttack : EnemyAttack
    {
        [SerializeField] public EnemyAttack attack;
        [HideInInspector] public List<EnemyAttack> listOfOrchestratedAttacks;
    }
}
