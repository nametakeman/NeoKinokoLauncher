using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModeChanger : MonoBehaviour
{
    [SerializeField] GameObject MainUI;
    [SerializeField] GameObject CommandMenu;
    [SerializeField] GameObject LoadingPanel;
    [SerializeField] GameObject UploadPanel;

    /// <summary>
    /// モードを変更する
    /// </summary>
    /// <param name="_key">アクティブにするモードのキーワード,main,command,loading,upload</param>
   public void SwitchMode(string _key)
    {
        MainUI.SetActive(false);
        CommandMenu.SetActive(false);
        LoadingPanel.SetActive(false);
        UploadPanel.SetActive(false);

        switch (_key)
        {
            case "main": 
                MainUI.SetActive(true);
                break;
            case "command":
                CommandMenu.SetActive(true);
                break;
            case "loading":
                LoadingPanel.SetActive(true);
                break;
            case "upload":
                UploadPanel.SetActive(true);
                break;
        }

    } 
   
}
