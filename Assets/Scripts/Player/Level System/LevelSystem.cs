using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LevelSystem 
{
    public event EventHandler OnExpChange;
    public event EventHandler OnLevelChange;


    private int level;
    private int exp;
    private int expToNextLevel;

    public LevelSystem() 
    {
        level = 0;
        exp = 0;
        expToNextLevel = 100;
    }

    public void AddExp(int amount) 
    { 
        exp += amount; 
        if (exp >= expToNextLevel)
        {
            level++;
            exp -= expToNextLevel;
            if (OnLevelChange != null)
            {
                OnLevelChange(this, EventArgs.Empty);
            }
        }
        if (OnExpChange != null)
        {
            OnExpChange(this, EventArgs.Empty);
        }
    }

    public int GetLevel()
    {
        return level;
    }

}
