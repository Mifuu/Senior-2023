using UnityEngine;
using System.Collections.Generic;

namespace Enemy
{
    public abstract class OrchestrationAttack : EnemyAttack
    {
        [SerializeField] public EnemyAttack attack;
        [SerializeField] public bool instantiateAttackForOrchestrator;
        protected EnemyAttack instantiatedAttack; // For used by the orchestrator

        private WeightedOrchestratorAttackStateSO controllerState;
        private int currentAttackControllerIndex;

        public override void Initialize(GameObject targetPlayer, GameObject enemyGameObject, DamageCalculationComponent component)
        {
            base.Initialize(targetPlayer, enemyGameObject, component);
            if (!instantiateAttackForOrchestrator) return;
            instantiatedAttack = Instantiate(attack); // BUG: Some attack is not appropriate to be init at the orchestrator
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
