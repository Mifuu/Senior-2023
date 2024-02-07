using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLevel : MonoBehaviour
{
    private LevelSystem levelSystem;

    public void SetLevelSystem(LevelSystem levelSystem)
    {
        this.levelSystem = levelSystem;
        levelSystem.OnLevelChange += LevelSystem_OnlevelChanged;
    }

    // call this to add exp to the player
    public void AddExp(int amount)
    {
        levelSystem.AddExp(amount);
    }

    private void LevelSystem_OnlevelChanged(object sender, EventArgs e)
    {
        Debug.Log("PlayerLevel Script: player is leveled up to lv " + levelSystem.GetLevel() );
    }


}
