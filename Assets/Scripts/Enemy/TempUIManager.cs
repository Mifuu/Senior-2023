using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
