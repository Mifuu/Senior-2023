using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerLevel : NetworkBehaviour
{
    private LevelSystem levelSystem;
    private PlayerHealth playerHealth;

    private void Awake()
    {
        playerHealth = GetComponent<PlayerHealth>();
    }

    public void SetLevelSystem(LevelSystem levelSystem)
    {
        this.levelSystem = levelSystem;
        levelSystem.OnLevelChange += LevelSystem_OnlevelChanged;
    }

    // call this to add exp to the player
    public void AddExp(float amount)
    {
        levelSystem.AddExp(amount);
    }

    private void LevelSystem_OnlevelChanged(object sender, EventArgs e)
    {
        Debug.Log("PlayerLevel Script: player is leveled up to lv " + levelSystem.GetLevel() );
        levelSystem.IncreaseExpToNextLevel();
        playerHealth.maxHealth += 1;
    }


}
