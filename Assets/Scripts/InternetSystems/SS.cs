using Cysharp.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Google.Apis.Sheets.v4.Data;

public class SS
{
    //�V�[�g�T�[�r�X���i�[�ł��A�p�u���b�N�Ŏ擾�ł���悤��
    public SheetsService _sheetsService { get; private set; }

    /// <summary>
    /// �X�v���b�h�V�[�g�ڑ��p��API���쐬
    /// </summary>
    /// <returns></returns>
    public async UniTask _createAPI()
    {
        if(_sheetsService != null)
        {
            return;
        }

        //�F�؏����擾����
        GoogleCredential _credential;
        using (var stream = new FileStream(new InternetDatas().JSON_FILE_PATH, FileMode.Open, FileAccess.Read)) 
        {
            _credential = GoogleCredential.FromStream(stream).CreateScoped(SheetsService.ScopeConstants.Spreadsheets);
        }

        //�X�v���b�h�V�[�g�̃T�[�r�X���쐬
        _sheetsService = new SheetsService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = _credential,
            ApplicationName = "Spread Sheet",
        });

    }

    /// <summary>
    /// �X�v���b�h�V�[�g�ɏ������ރX�N���v�g
    /// </summary>
    /// <param name="_strings">�����珇�Ԃɂ��̂܂܏�������</param>
    /// <returns></returns>
    public async UniTask writingSS(string[] _strings)
    {
        await UniTask.SwitchToThreadPool();

        List<object> list = new List<object>();
        foreach(string s in _strings)
        {
            list.Add(s);
        }
        list.Add(0);

        var wv = new List<IList<object>>()
        {
            list
        };
        Debug.Log("�L���f�[�^�̊i�[���������܂���");

        var body = new ValueRange() { Values = wv };
        var req = _sheetsService.Spreadsheets.Values.Append(body, new InternetDatas().SS_ID, "�V�[�g1!A1");
        try
        {
            req.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
        }
        catch(Exception e)
        {
            await UniTask.SwitchToMainThread();
            throw (e);
        }

        var result = req.Execute();
        Debug.Log(result);
        Debug.Log("�X�v���b�h�V�[�g�ւ̏������݂�����");

        await UniTask.SwitchToMainThread();
    }
}
