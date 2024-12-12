using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ローカル上のファイルパスを取得する。
/// </summary>
public class LocalDirPaths
{
    //ゲームが保存されるディレクトリ
    public string _gameFilePath { get; private set; } = "Downloaded";
    //jsonFileが保存されるディレクトリ
    public string _jsonFolderPath { get; private set; } = "JsonData";
}
