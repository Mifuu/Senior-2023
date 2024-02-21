using System.Collections.Generic;
using UnityEngine;
using System;

namespace Enemy
{
    [CreateAssetMenu(fileName = "EnemyGunWeaknessRule", menuName = "Enemy/Enemy Stat/Gun Weakness Rule")]
    public class EnemyGunWeaknessSO : ScriptableObject
    {
        [Serializable]
        public struct GunWeaknessFactorStruct
        {
            [SerializeField] public TemporaryGunType gunType;
            [SerializeField] public float factor;
        }

        [SerializeField] public List<GunWeaknessFactorStruct> gunWeaknessesList;

        private List<float> gunWeaknessListInternal;
        private bool isInitialized = false;

        public float GetGunWeaknessFactor(TemporaryGunType gunType)
        {
            if (!isInitialized) Initialize();
            return gunWeaknessListInternal[(int)gunType];
        }

        public void Initialize()
        {
            gunWeaknessListInternal = new List<float>(new float[Enum.GetNames(typeof(TemporaryGunType)).Length]);
            foreach (GunWeaknessFactorStruct gunWeakness in gunWeaknessesList)
            {
                gunWeaknessListInternal[(int)gunWeakness.gunType] = gunWeakness.factor;
            }

            isInitialized = true;
        }
    }
}
