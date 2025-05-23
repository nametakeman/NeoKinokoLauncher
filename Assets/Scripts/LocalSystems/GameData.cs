using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

[Serializable]
public class GameData
{
    public string FileName;
    public string GameName;
    //ここは拡張子までいれた名前を代入する
    public string ExeName;
    public string DevName;
    public string DriveId;
    public string ImageId;
    public string SoftwareType;
    public string Description;
    public string Version;
    public string Status;
}
