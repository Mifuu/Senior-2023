using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LevelSystem 
{
    public event EventHandler OnExpChange;
    public event EventHandler OnLevelChange;
    public event EventHandler OnSkillCardPointChange;


    private int level;
    private float exp;
    private float expToNextLevel;
    private float expToNextLevelScaleFactor;
    private int skillCardPoint;

    public LevelSystem() 
    {
        level = 1;
        exp = 0f;
        expToNextLevel = 100f;
        expToNextLevelScaleFactor = 1.05f;
        skillCardPoint = 0;
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

    public void AddSkillCardPoint (int amount)
    {
        skillCardPoint += amount;
        OnSkillCardPointChange?.Invoke(this, EventArgs.Empty);
    }

    public int GetLevel()
    {
        return level;
    }

    public float GetExpNormalized()
    {
        return (float)exp / expToNextLevel;
    }

    public int GetSkillCardPoint()
    {
        return skillCardPoint;
    }

    public void IncreaseExpToNextLevel()
    {
        expToNextLevel *= expToNextLevelScaleFactor;
        expToNextLevelScaleFactor += 0.005f;
    }

}
