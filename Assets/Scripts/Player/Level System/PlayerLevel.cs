using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerLevel : NetworkBehaviour
{
    public LevelSystem levelSystem;
    private PlayerHealth playerHealth;

    private void Awake()
    {
        playerHealth = GetComponent<PlayerHealth>();
        levelSystem = new LevelSystem();
        levelSystem.OnLevelChange += LevelSystem_OnlevelChanged;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            AddExp(5000);
        }
    }

    // call this to add exp to the player
    public void AddExp(float amount)
    {
        levelSystem.AddExp(amount);
    }

    [ContextMenu("Add 100 EXP")]
    public void Add100Exp()
    {
        AddExp(100f);
    }

    private void LevelSystem_OnlevelChanged(object sender, EventArgs e)
    {
        Debug.Log("PlayerLevel Script: player is leveled up to lv " + levelSystem.GetLevel());
        levelSystem.IncreaseExpToNextLevel();
        levelSystem.AddSkillCardPoint(1);
        playerHealth.BaseMaxHealth.Value += 2;
    }
}
