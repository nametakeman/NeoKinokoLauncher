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
    //シートサービスを格納でき、パブリックで取得できるように
    public SheetsService _sheetsService { get; private set; }

    /// <summary>
    /// スプレッドシート接続用のAPIを作成
    /// </summary>
    /// <returns></returns>
    public async UniTask _createAPI()
    {
        if(_sheetsService != null)
        {
            return;
        }

        //認証情報を取得する
        GoogleCredential _credential;
        using (var stream = new FileStream(new InternetDatas().JSON_FILE_PATH, FileMode.Open, FileAccess.Read)) 
        {
            _credential = GoogleCredential.FromStream(stream).CreateScoped(SheetsService.ScopeConstants.Spreadsheets);
        }

        //スプレッドシートのサービスを作成
        _sheetsService = new SheetsService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = _credential,
            ApplicationName = "Spread Sheet",
        });

    }

    /// <summary>
    /// スプレッドシートに書き込むスクリプト
    /// </summary>
    /// <param name="_strings">左から順番にそのまま書き込む</param>
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
        Debug.Log("記入データの格納が完了しました");

        var body = new ValueRange() { Values = wv };
        var req = _sheetsService.Spreadsheets.Values.Append(body, new InternetDatas().SS_ID, "シート1!A1");
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
        Debug.Log("スプレッドシートへの書き込みが完了");

        await UniTask.SwitchToMainThread();
    }
}
