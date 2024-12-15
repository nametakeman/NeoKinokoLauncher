using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ���O���擾���ďꍇ�ɉ����ēK�����ꏊ�ɕ\��������R�}���h
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
