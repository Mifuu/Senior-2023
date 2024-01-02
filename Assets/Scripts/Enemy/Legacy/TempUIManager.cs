using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace LegacyEnemy
{
    [Obsolete("Deprecated: This codes belong to old enemy codebase")]
    public class TempUIManager : MonoBehaviour
    {
        [SerializeField] private Button spawnButton;

        void Start()
        {
            spawnButton.onClick.AddListener(() =>
            {
                EnemySpawnManager.Instance.SetIsSpawn(true);
            });
        }
    }
}
