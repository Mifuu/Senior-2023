using UnityEngine;
using System.Collections.Generic;

namespace Enemy
{
    public abstract class OrchestrationAttack : EnemyAttack
    {
        [SerializeField] public EnemyAttack attack;
        protected EnemyAttack instantiatedAttack; // For used by the orchestrator

        private WeightedOrchestratorAttackStateSO controllerState;
        private int currentAttackControllerIndex;


        public override void Initialize(GameObject targetPlayer, GameObject enemyGameObject)
        {
            base.Initialize(targetPlayer, enemyGameObject);
            instantiatedAttack = Instantiate(attack);
            instantiatedAttack.Initialize(targetPlayer, enemyGameObject);
        }

        public void AssignOrchestrationController(WeightedOrchestratorAttackStateSO controllerState, int attackIndex)
        {
            this.controllerState = controllerState;
            this.currentAttackControllerIndex = attackIndex;
        }

        public List<EnemyAttack> GetAttacks() => controllerState.GetAllAttacksFromIndex(currentAttackControllerIndex);
    }
}
