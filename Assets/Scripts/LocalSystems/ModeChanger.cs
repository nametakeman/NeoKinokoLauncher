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
    /// ���[�h��ύX����
    /// </summary>
    /// <param name="_key">�A�N�e�B�u�ɂ��郂�[�h�̃L�[���[�h,main,command,loading,upload</param>
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
