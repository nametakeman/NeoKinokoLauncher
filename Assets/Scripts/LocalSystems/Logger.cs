using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ログを取得して場合に応じて適した場所に表示させるコマンド
/// </summary>
public class Logger : MonoBehaviour
{
    [SerializeField] Text _loadingLogTxt;

    private void Start()
    {
        Application.logMessageReceived += OnReceiveLog;
    }

    private void OnReceiveLog(string logText, string stackTrace, LogType logType)
    {
        _loadingLogTxt.text = logText;
    }
}
