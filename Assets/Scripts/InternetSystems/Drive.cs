using Cysharp.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Upload;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Drive
{
    //ドライブサービスを格納でき、パブリックから取得できるように作成。なければ作る
    public DriveService _driveService {
        get 
        {
            if (_driveService == null)
            {
                _driveService = _createAPI();
            }
            return _driveService;
        }
        private set { _driveService = value; }
    }



    /// <summary>
    /// インターネット接続用のAPIを作成。詳細はApisのリファレンス
    /// </summary>
    public DriveService _createAPI()
    {
        //ここらへんはマジで説明ムズイから公式のリファレンス読んでくれーーー

        //driveサービスの認証情報を取得する
        GoogleCredential credential;
        using (var stream = new FileStream(new InternetDatas().JSON_FILE_PATH, FileMode.Open, FileAccess.Read))
        {
            credential = GoogleCredential.FromStream(stream).CreateScoped(DriveService.ScopeConstants.Drive);
        }

        //DriveApiのサービスを作成
        DriveService _service = new DriveService( new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = "DriveService"
        } );

        return _service;
    }



    public async UniTask <string[]> DriveList(DriveService _ds)
    {
        List<string> _driveList = new List<string>();

        //フォルダ内の検索
        var _request = _ds.Files.List();
        _request.Q = "'" + new InternetDatas().JSON_FOLDER_ID + "' in parents";
        //ここで取得するフィールドをしていできる。idとかnameとか
        _request.Fields = "nextPageToken, files(id,name)";
        var files = new List<Google.Apis.Drive.v3.Data.File>();

        do
        {
            var result = _request.Execute();
            files.AddRange(result.Files);
            _request.PageToken = result.NextPageToken;
        } while (!string.IsNullOrEmpty(_request.PageToken));

        

        //結果を出力する
        foreach (var file in files)
        {
            _driveList.Add(file.Id);
        }

        return _driveList.ToArray();
    }

    /// <summary>
    /// 指定されたIDのファイルをダウンロードする
    /// </summary>
    /// <param name="_id">DriveID</param>
    /// <param name="_path">保存先のパス</param>
    /// <returns></returns>
    async UniTask DlFile(DriveService _service, string _id, string _path)
    {
        //スレッドをメインスレッドから切り替え
        await UniTask.SwitchToThreadPool();

        //メタデータの取得
        var file = _service.Files.Get(_id).Execute();
        if(file == null)
        {
            await UniTask.SwitchToMainThread();
            //例外を投げる
            throw new System.Exception("failed get meta");
        }

        //本ファイルのダウンロード
        var request = _service.Files.Get(_id);
        var fileStream = new FileStream(Path.Combine(_path, file.Name), FileMode.Create, FileAccess.Write);
        request.Download(fileStream);
        fileStream.Close();
        await UniTask.SwitchToMainThread();
    }
}
