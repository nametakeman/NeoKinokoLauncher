using Ookii.Dialogs;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class UploadingPanel : MonoBehaviour
{
    [SerializeField] GameObject GUIProcess;

    [SerializeField] Text _gameNameTx;
    [SerializeField] Text _folderPathTx;
    [SerializeField] Text _imagePathTx;
    [SerializeField] Text _exeNameTx;
    [SerializeField] Text _devNameTx;
    [SerializeField] Text _softwareTx;
    [SerializeField] Text _descriptionTx;

    public void pushedSelectFile()
    {
        string _filePath = new openSelectedFile().openFolderDialog();
        if(_filePath == "" || _filePath == "error")
        {
            return;
        }
        _folderPathTx.text = _filePath;
    }

    public void pushedSelectImage()
    {
        string _filePath = new openSelectedFile().openFileDialogImage();
        if (_filePath == "" || _filePath == "error")
        {
            return;
        }
        _imagePathTx.text = _filePath;
    }

    public void pushedExe()
    {
        string _filePath = new openSelectedFile().openFileDialogAny();
        if (_filePath == "" || _filePath == "error")
        {
            return;
        }
        _exeNameTx.text = _filePath;
    }

    public void pushedDownload()
    {
        string _folderPath = _folderPathTx.text;
        if (_folderPath == "")
        {
            Debug.Log("ゲームフォルダを選択してください");
            return;
        }

        if(_exeNameTx.text == "実行ファイルを選択してください")
        {
            Debug.Log("実行ファイルを選択してください");
            return;
        }

        string[] _exeNameArray = _exeNameTx.text.Split(@"\");
        string _exeName = _exeNameArray[_exeNameArray.Length - 1];

        GUIProcess.GetComponent<GUIProcess>().UploadProcessing(_folderPath, _gameNameTx.text, _exeName, _devNameTx.text, _softwareTx.text, _descriptionTx.text, _imagePathTx.text);

        _exeNameTx.text = "実行ファイルを選択してください";
    }
}
