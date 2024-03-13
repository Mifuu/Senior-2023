using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace Enemy
{
    [CreateAssetMenu(menuName = "Enemy/Enemy State/Attack State/Weighted Attack", fileName = "WeightedAttack")]
    public class WeightAttackPoolAttackStateSO : EnemyAttackSOBase
    {
        [Header("Weighted Attack Parameter")]
        [SerializeField] public List<WeightedEnemyAttack> weightedAttacks = new List<WeightedEnemyAttack>();
        [SerializeField] private EnemyAttack emptyAttack;
        [SerializeField] private bool autoAdjustWeight = true;

        [Header("Other Config")]
        [SerializeField] private float maximumStateHoldingTime = 10.0f;
        [SerializeField] private int maximumConsecutiveAttack = 3;
        [SerializeField] private int maximumSearchIteration = 10;

        private int previousAttackIndex = 0;
        private int consecutiveAttack = 0;

        public override void Initialize(GameObject gameObject, EnemyBase enemy)
        {
            base.Initialize(gameObject, enemy);
            this.weightedAttacks = this.weightedAttacks.Select((attack) =>
            {
                attack = Instantiate(attack);
                attack.Initialize(enemy.targetPlayer, gameObject);
                return attack;
            }).ToList();
        }

        // Check if the attack is ready: 
        //  if return PROCEED, switch to selected attack 
        //  if return HOLD, hold the state and ask for callback
        //  if return REPICK, remove attack from the pool and do another random 
        //      until there's no attack left, then skip
        //  if return SKIP, change state
        public void ProcessAttack()
        {
            // Debug.LogWarning("**************************PROCESSING ATTACK**************************");
            var listOfAttackIndex = GenerateRandomAttackIndexPool();
            var randomProcessSucceed = false;
            int currentSeachIteration = 0;

            while (!randomProcessSucceed && listOfAttackIndex.Count != 0)
            {
                // Possible BUG, list can be empty when SelectRandomAttack is called
                var selectedAttackIndex = SelectRandomAttack(listOfAttackIndex);
                currentSeachIteration++;

                // Debug.Log("Pool Count: = " + listOfAttackIndex.Count + ", " + (listOfAttackIndex.Count == 0));
                // Debug.Log("Pool: " + DebugPrintList(listOfAttackIndex));
                // Debug.Log("Max Reach: " + currentSeachIteration + ", " + (currentSeachIteration >= maximumSearchIteration));

                // Check if no more available attack as well as prevent too many search iteration
                if (listOfAttackIndex.Count == 0 || currentSeachIteration >= maximumSearchIteration)
                {
                    // Debug.LogError("-------------------------------------");
                    // Debug.LogError("Search Exits");
                    // Debug.LogError("Available Attack: " + listOfAttackIndex.Count);
                    // Debug.LogError("Search Iteration: " + currentSeachIteration);
                    // foreach (var attack in weightedAttacks)
                    // {
                    //     Debug.LogError(attack + "'s current weight: " + attack.currentWeight);
                    // }
                    // Debug.LogError("Resetting Weight");
                    // Debug.LogError("-------------------------------------");

                    ResetWeight();
                    break;
                }

                // Prevent too many same consecutive attack
                if (previousAttackIndex == selectedAttackIndex)
                {
                    consecutiveAttack++;
                    if (consecutiveAttack >= maximumConsecutiveAttack)
                    {
                        // Debug.LogWarning("Too much Consecutive");
                        // Debug.LogWarning("Removing: " + selectedAttackIndex);
                        listOfAttackIndex = RemoveAttackFromRandomPool(listOfAttackIndex, selectedAttackIndex);
                        // Debug.LogWarning("List: " + DebugPrintList(listOfAttackIndex));
                        continue;
                    }
                }
                else
                {
                    consecutiveAttack = 0;
                    previousAttackIndex = selectedAttackIndex;
                }

                var response = weightedAttacks[selectedAttackIndex].CheckAndActivateAttack();
                switch (response)
                {
                    case EnemyWeightedAttackResponseMode.Proceed:
                        Debug.LogWarning("Weighted Attack Proceeding " + weightedAttacks[selectedAttackIndex]);
                        SwitchAttack(selectedAttackIndex);
                        randomProcessSucceed = true;
                        break;
                    case EnemyWeightedAttackResponseMode.Hold:
                        Debug.LogWarning("Weighted Attack Holding " + weightedAttacks[selectedAttackIndex]);
                        RequestStateHoldingCallback(selectedAttackIndex);
                        randomProcessSucceed = true;
                        break;
                    case EnemyWeightedAttackResponseMode.Repick:
                        Debug.LogWarning("Weighted Attack Repicking " + weightedAttacks[selectedAttackIndex]);
                        listOfAttackIndex = RemoveAttackFromRandomPool(listOfAttackIndex, selectedAttackIndex);
                        // Debug.LogWarning("Removing: " + selectedAttackIndex);
                        // Debug.LogWarning("List: " + DebugPrintList(listOfAttackIndex));
                        break;
                    case EnemyWeightedAttackResponseMode.Skip:
                        Debug.LogWarning("Weighted Attack Skipping " + weightedAttacks[selectedAttackIndex]);
                        enemy.StateMachine.ChangeState(enemy.IdleState);
                        randomProcessSucceed = true;
                        return;
                }
            }

            if (!randomProcessSucceed)
                enemy.StateMachine.ChangeState(enemy.IdleState);
        }

        private List<int> RemoveAttackFromRandomPool(List<int> currentPool, int filter) => currentPool.Where((num) => num != filter).ToList();
        private int SelectRandomAttack(List<int> indexedList) => indexedList[Random.Range(0, indexedList.Count)];

        private List<int> GenerateRandomAttackIndexPool()
        {
            // Expensive, Maybe find a better way to do this
            List<int> indexedList = new List<int>();
            for (int i = 0; i < weightedAttacks.Count; i++)
            {
                for (int j = 0; j < weightedAttacks[i].currentWeight; j++)
                {
                    indexedList.Add(i);
                }
            }
            return indexedList;
        }

        private void SwitchAttack(int weightedAttackIndex)
        {
            var attack = weightedAttacks[weightedAttackIndex];
            attack.DecrementStaminaOnAttackUsed();
            if (autoAdjustWeight) AdjustWeight(weightedAttackIndex);

            this.selectedNextAttack = attack.attack;
            this.selectedNextAttack.PerformAttack();

            if (attack.requireAttackEnds)
            {
                void EndState()
                {
                    enemy.StateMachine.ChangeState(enemy.IdleState);
                    attack.attack.OnAttackEnds -= EndState;
                }

                attack.attack.OnAttackEnds += EndState;
                return;
            }

            enemy.StateMachine.ChangeState(enemy.IdleState);
        }

        #region Holding State Handler

        private IEnumerator RequestStateHoldingCallback(int weightedAttackIndex)
        {
            bool isHolding = true;
            void HandleHoldingCallback()
            {
                SwitchAttack(weightedAttackIndex);
                isHolding = false;
            }

            var attack = weightedAttacks[weightedAttackIndex];
            attack.GenerateHoldingCallback(HandleHoldingCallback);
            yield return new WaitForSeconds(maximumStateHoldingTime);
            if (isHolding) enemy.StateMachine.ChangeState(enemy.IdleState);
        }

        #endregion

        private void AdjustWeight(int weightedAttackIndex)
        {
            for (int i = 0; i < weightedAttacks.Count; i++)
            {
                if (i == weightedAttackIndex)
                {
                    weightedAttacks[i].DecrementCurrentWeight();
                }
                else
                {
                    weightedAttacks[i].IncrementCurrentWeight();
                }
            }
        }

        // Used in emergency, when all the attack has no weight
        private void ResetWeight()
        {
            // Debug.LogWarning("Resetting All Weight");
            foreach (var attack in weightedAttacks)
            {
                attack.ResetWeight();
            }
        }

        private string DebugPrintList(List<int> list)
        {
            string toBePrinted = "";
            foreach (var l in list)
            {
                toBePrinted += (l + ", ");
            }

            return toBePrinted;
        }

        public override void DoEnterLogic()
        {
            base.DoEnterLogic();
            this.selectedNextAttack = emptyAttack;
            ProcessAttack();
        }
    }

    public enum EnemyWeightedAttackResponseMode
    {
        Proceed, // Proceed with the current attack
        Hold, // Hold the attack state for some time until able to proceed with the attack
        Repick, // Temporarily remove the current attack from the random attack list, then randomly pick another attack, Skip if the list is empty
        Skip // Skip the attack state and proceed to Idle state, Do not adjust the stamina or weight
    }
}
