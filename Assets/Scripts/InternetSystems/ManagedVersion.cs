using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Text;

public class ManagedVersion : MonoBehaviour
{

    /// <summary>
    /// テキスト形式のファイルからVersionクラスに変更する
    /// </summary>
    /// <returns></returns>
    public Version[] TextToVersionClass()
    {
        string _filepath = new InternetDatas().VERSION_FILE_PATH;
        //読み取るテキストファイルがなければ例外を返して処理を終了
        if (!File.Exists(_filepath))
        {
            throw new System.Exception("inexists text file!");
        }

        StreamReader _stream = new StreamReader(_filepath, Encoding.UTF8);
        
        string _str = _stream.ReadToEnd();
        _stream.Close();
        _str = _str.Replace(Environment.NewLine, "\r");
        _str = _str.Trim();
        string[] _stringDatas = _str.Split("\r");

        List<Version> _returnData = new List<Version>();
        foreach(string s in _stringDatas)
        {
            string[] s2 = s.Split("#");
            Version v = new Version();
            foreach(string s3 in s2)
            {
                if (s3.Contains("Name"))
                {
                    v._gameName = s3.Replace("Name:", "");
                }
                else if (s3.Contains("Version"))
                {
                    v._gameVersion = s3.Replace("Version:", "");
                }
            }
            _returnData.Add(v);
        }
        return _returnData.ToArray();
    }
    
    /// <summary>
    /// Versionクラスを受け取ってフォーマット通りにテキストファイルを作成するクラス
    /// 既存ファイルを上書きするため、渡す配列に既存のバージョンデータを含めておく！
    /// </summary>
    private void ClassToText(Version[] _versions)
    {
        string _filepath = new InternetDatas().VERSION_FILE_PATH;

        string _targetStr = "";
        foreach(Version v in _versions)
        {
            _targetStr += "Name:" + v._gameName + "#";
            _targetStr += "Version:" + v._gameVersion + "\r";
        }
        _targetStr.Trim();

        using (StreamWriter sw = new StreamWriter(_filepath, false))
        {
            sw.WriteLine(_targetStr);
        }
    }
}

/// <summary>
/// テキストファイル側の記入フォーマット例
/// Name:<フォルダ名>#Version:<ゲームのバージョン>
/// Name:NonStopTuna2#Version:1
/// 上記はNonStopTuna2のバージョン1を表す。
/// 他ゲームの記入は必ず改行して記入する。
/// </summary>
public class Version
{
    public string _gameName { get; set; } = "";
    public string _gameVersion { get; set; } = "";
}
