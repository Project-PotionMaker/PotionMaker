using UnityEngine;
using System.Collections.Generic;

public class InGameLogViewer : MonoBehaviour
{
    private List<string> logMessages = new List<string>();
    private Vector2 scrollPosition = Vector2.zero;
    private bool showLog = false;
    private int maxLogs = 20; // 화면에 표시할 최대 로그 개수

    private void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    private void HandleLog(string logText, string stackTrace, LogType type)
    {
        if (logMessages.Count >= maxLogs)
        {
            logMessages.RemoveAt(0);
        }
        string formattedLog = $"[{System.DateTime.Now:HH:mm:ss}] {logText}";
        logMessages.Add(formattedLog);
    }

    private void Update()
    {
        // F1 키를 누르면 로그 창을 토글합니다.
        if (Input.GetKeyDown(KeyCode.F1))
        {
            showLog = !showLog;
        }
    }

    private void OnGUI()
    {
        if (!showLog)
            return;

        GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "");

        GUI.color = Color.white;

        GUILayout.BeginArea(new Rect(10, 10, Screen.width - 20, Screen.height - 20));
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        foreach (var log in logMessages)
        {
            GUILayout.Label(log);
        }

        GUILayout.EndScrollView();
        GUILayout.EndArea();
    }
}