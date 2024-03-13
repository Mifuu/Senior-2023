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
        [SerializeField] public int incrementWeightAmount;
        [SerializeField] public int decrementWeightAmount;
        [Header("Attack Process")]
        [SerializeField] public bool requireAttackEnds;

        [HideInInspector] public int currentWeight;
        protected BossStaminaManager staminaManager;

        public void Initialize(GameObject targetPlayer, GameObject enemy)
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

        public void IncrementCurrentWeight(int value)
        {
            currentWeight += value;
        }

        public void IncrementCurrentWeight() => IncrementCurrentWeight(incrementWeightAmount);

        public void DecrementCurrentWeight(int value)
        {
            currentWeight -= value;
        }

        public void DecrementCurrentWeight() => DecrementCurrentWeight(decrementWeightAmount);
    }
}
