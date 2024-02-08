using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Testing : MonoBehaviour
{
    //[SerializeField] private LevelWindow levelWindow;
    [SerializeField] private PlayerLevel playerLevel;
    private void Start()
    {
        LevelSystem levelSystem = new LevelSystem();
        playerLevel.SetLevelSystem(levelSystem);

        // Find the GameObject with the LevelWindow script attached
        GameObject levelWindowObject = GameObject.Find("LevelWindow");

        // Get the LevelWindow script from the GameObject
        LevelWindow levelWindow = levelWindowObject.GetComponent<LevelWindow>();

        // Set the level system for LevelWindow
        levelWindow.SetLevelSystem(levelSystem);
    }
}
