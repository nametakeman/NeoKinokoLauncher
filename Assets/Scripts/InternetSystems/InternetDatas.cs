using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// �C���^�[�l�b�g�ڑ��Ɏg�p���邽�߂̃f�[�^���i�[�����t�@�C��
/// </summary>
public class InternetDatas
{
    public string JSON_FILE_PATH { get; private set; } = UnityEngine.Application.streamingAssetsPath + "\\" + "path-to-key-json.json";
    public string JSON_FOLDER_ID { get; private set; } = "";
    public string VERSION_FILE_PATH { get; private set; } = "version\\version.txt";
}
