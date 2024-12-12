using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// インターネット接続に使用するためのデータを格納したファイル
/// </summary>
public class InternetDatas
{
    public string JSON_FILE_PATH { get; private set; } = UnityEngine.Application.streamingAssetsPath + "\\" + "path-to-key-json.json";
    public string JSON_FOLDER_ID { get; private set; } = "";
}
