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
    /// �e�L�X�g�`���̃t�@�C������Version�N���X�ɕύX����
    /// </summary>
    /// <returns></returns>
    public Version[] TextToVersionClass()
    {
        string _filepath = new InternetDatas().VERSION_FILE_PATH;
        //�ǂݎ��e�L�X�g�t�@�C�����Ȃ���Η�O��Ԃ��ď������I��
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
    /// Version�N���X���󂯎���ăt�H�[�}�b�g�ʂ�Ƀe�L�X�g�t�@�C�����쐬����N���X
    /// �����t�@�C�����㏑�����邽�߁A�n���z��Ɋ����̃o�[�W�����f�[�^���܂߂Ă����I
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
/// �e�L�X�g�t�@�C�����̋L���t�H�[�}�b�g��
/// Name:<�t�H���_��>#Version:<�Q�[���̃o�[�W����>
/// Name:NonStopTuna2#Version:1
/// ��L��NonStopTuna2�̃o�[�W����1��\���B
/// ���Q�[���̋L���͕K�����s���ċL������B
/// </summary>
public class Version
{
    public string _gameName { get; set; } = "";
    public string _gameVersion { get; set; } = "";
}
