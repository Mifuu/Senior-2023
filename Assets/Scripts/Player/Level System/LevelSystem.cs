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
        level = 1;
        exp = 0;
        expToNextLevel = 100;
    }

    public void AddExp(int amount) 
    {
        exp += amount; 
        while (exp >= expToNextLevel)
        {
            level++;
            exp -= expToNextLevel;
            OnLevelChange?.Invoke(this, EventArgs.Empty);
        }
        OnExpChange?.Invoke(this, EventArgs.Empty);
    }

    public int GetLevel()
    {
        return level;
    }

    public float GetExpNormalized()
    {
        return (float)exp / expToNextLevel;
    }

}
