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

    //ディレクトリが生成されているかの確認と生成されていないなら生成するメソッド
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
    
    //GameDataクラスの配列を作成して返す(jsonデータが存在するだけ作成)
    public async UniTask<GameData[]> _createGameData()
    {
        //jsonファイルのパスをディレクトリ下にあるだけ取得する
        string[] _jsonDatas = new string[0];
        //jsonファイルのパスを取得
        _jsonDatas = Directory.GetFiles(new LocalDirPaths()._jsonFolderPath, "*.json");

        //以下がjsonファイル→クラスへの処理
        List<GameData> _gameDataList = new List<GameData>();

        foreach(string path in _jsonDatas)
        {
            //パスからjsonをstring型にしてJsonUtilityからポコクラスに代入してリストに代入する
            _gameDataList.Add(JsonUtility.FromJson<GameData>(_readJSON(path)));
        }

        return _gameDataList.ToArray();
    }


    //jsonファイルの読み取りをするメソッド
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

    //ゲーム保存用のローカルディレクトリに入っているゲームのファイル名をリストにして保存する
    public string[] _createLocalGameFN()
    {
        string[] _folderName = Directory.GetDirectories(new LocalDirPaths()._gameFilePath);

        //整形
        List<string> _returnData = new List<string>();
        foreach(string s in _folderName)
        {
            _returnData.Add(s.Replace($"{new LocalDirPaths()._gameFilePath}\\",""));
        }

        return _returnData.ToArray();
        
    }

    //ローカルのみで追加されたゲームを含む全ゲームのGameDatas配列を作成する
    public async UniTask<GameData[]> _createAllGameDatas(GameData[] _onlineDatas, string[] _offlineDirNames)
    {
        List<GameData> _returnData = new List<GameData> (_onlineDatas);
        
        //jsonファイルが保存されているデータからフォルダ名のみを取得
        List<string> _gameDataFileNameArray = new List<string>();
        foreach (GameData g in _onlineDatas)
        {
            _gameDataFileNameArray.Add(g.FileName);
        }

        //_gameDataFileNameArrayと_offlineDirNameを比較しゲームデータがないものは新しくクラスを生成して追加する
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
    /// 渡されたGameData配列からゲームが実際にフォルダにあるかを確認して代入する
    /// </summary>
    public async UniTask _checkStatus(GameData[] _datas)
    {

        foreach (GameData g in _datas)
        {
            //未インストールの際
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
