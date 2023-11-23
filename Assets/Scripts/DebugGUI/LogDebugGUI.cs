using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogDebugGUI : MonoBehaviour
{
    uint queueSize = 16;
    Queue log = new Queue();

    void OnEnable() => Application.logMessageReceived += HandleLog;

    void OnDisable() => Application.logMessageReceived += HandleLog;

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        log.Enqueue($"[{type}] : {logString}");
        if (log.Count >= queueSize)
            log.Dequeue();
    }

    void OnGUI()
    {
        if (!DebugGUIManager.active)
            return;

        GUILayout.BeginArea(new Rect(Screen.width - 400, 10, 400, Screen.height / 3));
        GUILayout.Label("[LogDebugGUI]");
        GUILayout.FlexibleSpace();
        GUILayout.Label("\n" + string.Join("\n", log.ToArray()));
        GUILayout.EndArea();
    }
}
