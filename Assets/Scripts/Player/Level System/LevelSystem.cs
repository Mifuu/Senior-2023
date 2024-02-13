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
    private float exp;
    private float expToNextLevel;
    private float expToNextLevelScaleFactor;

    public LevelSystem() 
    {
        level = 1;
        exp = 0f;
        expToNextLevel = 100f;
        expToNextLevelScaleFactor = 1.1f;
    }

    public void AddExp(float amount) 
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

    public void IncreaseExpToNextLevel()
    {
        expToNextLevel *= expToNextLevelScaleFactor;
        expToNextLevelScaleFactor += 0.02f;
    }

}
