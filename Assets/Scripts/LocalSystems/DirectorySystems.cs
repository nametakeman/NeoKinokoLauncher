using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Text;
using TMPro;
using UnityEditor.Experimental.GraphView;

public class DirectorySystems
{

    //�f�B���N�g������������Ă��邩�̊m�F�Ɛ�������Ă��Ȃ��Ȃ琶�����郁�\�b�h
    public async UniTask _checkDirectorys()
    {
        string _gameFilePath = new LocalDirPaths()._gameFilePath;
        string _jsonFolderPath = new LocalDirPaths()._jsonFolderPath;

        if (!Directory.Exists(_gameFilePath))
        {
            Directory.CreateDirectory(_gameFilePath);
        }
        if(!Directory.Exists(_jsonFolderPath)) 
        {
            Directory.CreateDirectory(_jsonFolderPath);
        }
    }
    
    //GameData�N���X�̔z����쐬���ĕԂ�(json�f�[�^�����݂��邾���쐬)
    public async UniTask<GameData[]> _createGameData()
    {
        //json�t�@�C���̃p�X���f�B���N�g�����ɂ��邾���擾����
        string[] _jsonDatas = new string[0];
        //json�t�@�C���̃p�X���擾
        _jsonDatas = Directory.GetFiles(new LocalDirPaths()._jsonFolderPath, "*.json");

        //�ȉ���json�t�@�C�����N���X�ւ̏���
        List<GameData> _gameDataList = new List<GameData>();

        foreach(string path in _jsonDatas)
        {
            //�p�X����json��string�^�ɂ���JsonUtility����|�R�N���X�ɑ�����ă��X�g�ɑ������
            _gameDataList.Add(JsonUtility.FromJson<GameData>(_readJSON(path)));
        }

        return _gameDataList.ToArray();
    }


    //json�t�@�C���̓ǂݎ������郁�\�b�h
    private string _readJSON(string _path)
    {
        string _jsonData = "";
        try
        {
            using(var fs = new FileStream(_path, FileMode.OpenOrCreate))
            {
                using(var sr = new StreamReader(fs))
                {
                    _jsonData = sr.ReadToEnd();
                }
            }
        }
        catch (Exception e) 
        {
            Debug.Log(e);
        }
        return _jsonData;
    }

    //�Q�[���ۑ��p�̃��[�J���f�B���N�g���ɓ����Ă���Q�[���̃t�@�C���������X�g�ɂ��ĕۑ�����
    public string[] _createLocalGameFN()
    {
        string[] _folderName = Directory.GetDirectories(new LocalDirPaths()._gameFilePath);

        //���`
        List<string> _returnData = new List<string>();
        foreach(string s in _folderName)
        {
            _returnData.Add(s.Replace($"{new LocalDirPaths()._gameFilePath}\\",""));
        }

        return _returnData.ToArray();
        
    }

    //���[�J���݂̂Œǉ����ꂽ�Q�[�����܂ޑS�Q�[����GameDatas�z����쐬����
    public async UniTask<GameData[]> _createAllGameDatas(GameData[] _onlineDatas, string[] _offlineDirNames)
    {
        List<GameData> _returnData = new List<GameData> (_onlineDatas);
        
        //json�t�@�C�����ۑ�����Ă���f�[�^����t�H���_���݂̂��擾
        List<string> _gameDataFileNameArray = new List<string>();
        foreach (GameData g in _onlineDatas)
        {
            _gameDataFileNameArray.Add(g.FileName);
        }

        //_gameDataFileNameArray��_offlineDirName���r���Q�[���f�[�^���Ȃ����̂͐V�����N���X�𐶐����Ēǉ�����
        foreach (string s in _offlineDirNames)
        {
            if (!_gameDataFileNameArray.Contains(s))
            {
                GameData g = new GameData();
                g.FileName = s;
                _returnData.Add(g);
            }
        }

        return _returnData.ToArray();
    }

    /// <summary>
    /// �n���ꂽGameData�z�񂩂�Q�[�������ۂɃt�H���_�ɂ��邩���m�F���đ������
    /// </summary>
    public async UniTask _checkStatus(GameData[] _datas)
    {

        foreach (GameData g in _datas)
        {
            //���C���X�g�[���̍�
            if (!Directory.Exists(new LocalDirPaths()._gameFilePath + "\\" + g.FileName))
            {
                Debug.Log("NonInstall");
                g.Status = "NotInstall";
                continue;
            }
            else if (!File.Exists(new LocalDirPaths()._jsonFolderPath + "\\" + g.FileName + ".json"))
            {
                Debug.Log("Local");
                g.Status = "Local";
                continue;
            }
            else
            {
                Debug.Log("Online");
                g.Status = "Online";
                continue;
            }
        }
    }
}
