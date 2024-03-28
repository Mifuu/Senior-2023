using UnityEngine;
using System;

namespace Enemy
{
    [Serializable]
    public abstract class WeightedEnemyAttack : ScriptableObject
    {
        [Header("Wrapped Attack")]
        [SerializeField] public EnemyAttack attack;
        [Header("Stamina")]
        [SerializeField] public int requiredStamina;
        [Header("Probability Weight")]
        [SerializeField] public int startingWeight;
        [SerializeField] public int maximumWeight = 10;
        [SerializeField] public int minimumWeight = 0;
        [SerializeField] public int incrementWeightAmount;
        [SerializeField] public int decrementWeightAmount;
        [Header("Attack Process")]
        [SerializeField] public bool requireAttackEnds;
        [Header("Stamina Bonus")]
        [SerializeField] public int staminaBonus = 0;

        [HideInInspector] public int currentWeight;
        protected BossStaminaManager staminaManager;
        [HideInInspector] public WeightAttackPoolAttackStateSO controller;

        public virtual void Initialize(GameObject targetPlayer, GameObject enemy)
        {
            currentWeight = startingWeight;
            attack = Instantiate(attack);
            attack.Initialize(targetPlayer, enemy);
            staminaManager = enemy.GetComponent<BossStaminaManager>();
        }

        public abstract EnemyWeightedAttackResponseMode CheckAndActivateAttack();

        public virtual void GenerateHoldingCallback(Action callback)
        {
            callback();
            throw new Exception("Please specify GenerateHoldingCallback function if Attack state holding is used");
        }

        public void ResetWeight() => currentWeight = startingWeight;

        public void DecrementStaminaOnAttackUsed() => staminaManager.DecrementStamina(requiredStamina);

        public void ApplyStaminaBonus() => staminaManager.IncrementStamina(staminaBonus);

        public int IncrementCurrentWeight(int value)
        {
            int temp = currentWeight;
            currentWeight = Math.Clamp(currentWeight + value, minimumWeight, maximumWeight);
            return currentWeight - temp;
        }

        public int IncrementCurrentWeight() => IncrementCurrentWeight(incrementWeightAmount);

        public int DecrementCurrentWeight(int value)
        {
            int temp = currentWeight;
            currentWeight -= Math.Clamp(currentWeight - value, minimumWeight, maximumWeight);
            return temp - currentWeight;
        }

        public int DecrementCurrentWeight() => DecrementCurrentWeight(decrementWeightAmount);
    }
}
