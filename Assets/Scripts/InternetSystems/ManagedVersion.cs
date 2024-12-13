using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Text;

public class ManagedVersion : MonoBehaviour
{
    private void Start()
    {
        Version[] versions = TextToVersionClass();
        foreach (Version version in versions)
        {
            Debug.Log(version._gameName);
        }
    }
    
    /// <summary>
    /// テキスト形式のファイルからVersionクラスに変更する
    /// </summary>
    /// <returns></returns>
    private Version[] TextToVersionClass()
    {
        string _filepath = new InternetDatas().VERSION_FILE_PATH;
        //読み取るテキストファイルがなければ例外を返して処理を終了
        if (!Directory.Exists(_filepath))
        {
            throw new System.Exception("inexists text file!");
        }

        StreamReader _stream = new StreamReader(_filepath, Encoding.UTF8);

        string _str = _stream.ReadToEnd();
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
    

}

/// <summary>
/// テキストファイル側の記入フォーマット例
/// Name:<フォルダ名>#Game:<ゲームのバージョン>
/// Name:NonStopTuna2#Version:1
/// 上記はNonStopTuna2のバージョン1を表す。
/// 他ゲームの記入は必ず改行して記入する。
/// </summary>
public class Version
{
    public string _gameName { get; set; } = "";
    public string _gameVersion { get; set; } = "";
}
