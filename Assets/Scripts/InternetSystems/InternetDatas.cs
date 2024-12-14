using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// インターネット接続に使用するためのデータを格納したファイル
/// </summary>
public class InternetDatas
{
    public string JSON_FILE_PATH { get; private set; } = UnityEngine.Application.streamingAssetsPath + "\\" + "path-to-key-json.json";
    public string JSON_FOLDER_ID { get; private set; } = "1OwwmDlJ9k4R8F9dmA4PA56dyelL3Ck_I";
    public string VERSION_FILE_PATH { get; private set; } = "version\\version.txt";
}
