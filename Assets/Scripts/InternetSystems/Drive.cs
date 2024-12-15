using Cysharp.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Google.Apis.Upload;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Drive
{
    //ドライブサービスを格納でき、パブリックから取得できるように作成。なければ作る
    public DriveService _driveService { get; private set; }



    /// <summary>
    /// インターネット接続用のAPIを作成。詳細はApisのリファレンス
    /// </summary>
    public async UniTask _createAPI()
    {
        if(_driveService != null)
        {
            return;
        }
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

        _driveService =  _service;
    }

    /// <summary>
    /// すべてのjsonファイルをDLする。
    /// </summary>
    /// <returns>false=失敗</returns>
    public async UniTask<bool> DlAllJson()
    {
        if (_driveService == null)
        {
            Debug.Log("APIが作成されていません<DlAllJson>");
            return false;
        }
        //ドライブに保存されているjsonデータをidの配列で取得してくる。ここが無理な場合はネットに接続されていないものとする。
        string[] _strA = null;
        try
        {
            _strA = await DriveList();
        }
        catch (System.Exception e)
        {
            Debug.Log("インターネットに接続されていません\rエラー内容:" + e);
            return false;
        }

        string _dirPathJ = new LocalDirPaths()._jsonFolderPath;
        int _counter = 0;
        foreach (string s in _strA)
        {
            Debug.Log($"jsonファイルをダウンロード中({_counter}/{_strA.Length})");
            try
            {
                await DlFile(s, _dirPathJ);

            }
            catch (System.Exception e)
            {
                Debug.Log("ダウンロードに失敗しました。\rエラー内容：" + e);
                return false;
            }
            _counter++;
        }

        return true;
    }

    /// <summary>
    /// ゲームをダウンロードするための関数
    /// </summary>
    /// <returns></returns>
    public async UniTask<bool> DlGame(GameData _data)
    {
        //APIが作成されているかの確認
        if (_driveService == null)
        {
            Debug.Log("APIが作成されていません<DlAllJson>");
            return false;
        }

        if(_data.DriveId == "")
        {
            Debug.Log("DriveIdが代入されていません");
            return false;
        }

        try
        {
            await DlFile(_data.DriveId, new LocalDirPaths()._gameFilePath);
        }
        catch (System.Exception e)
        {
            Debug.Log("ゲームのダウンロードに失敗しました。\rエラー内容：" + e);
            return false;
        }

        try
        {
            await ExtractZIP(new LocalDirPaths()._gameFilePath + "\\" + _data.FileName + ".zip");
        }
        catch (System.Exception e)
        {
            Debug.Log("zipファイルの解凍に失敗しました。\rエラー内容：" + e);
        }

        Debug.Log("ゲームのダウンロードに成功");
        return true;
    }



    async UniTask <string[]> DriveList()
    {
        List<string> _driveList = new List<string>();

        //フォルダ内の検索
        var _request = _driveService.Files.List();
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
    async UniTask DlFile(string _id, string _path)
    {
        //スレッドをメインスレッドから切り替え
        await UniTask.SwitchToThreadPool();

        //メタデータの取得
        var file = _driveService.Files.Get(_id).Execute();
        if(file == null)
        {
            await UniTask.SwitchToMainThread();
            //例外を投げる
            throw new System.Exception("failed get meta");
        }

        //本ファイルのダウンロード
        var request = _driveService.Files.Get(_id);
        var fileStream = new FileStream(Path.Combine(_path, file.Name), FileMode.Create, FileAccess.Write);
        request.Download(fileStream);
        fileStream.Close();
        await UniTask.SwitchToMainThread();
    }

    /// <summary>
    /// 指定されたパスのファイルを解凍する、元のzipファイルは削除される
    /// </summary>
    /// <param name="_path"></param>
    /// <returns></returns>
    async UniTask ExtractZIP(string _path)
    {
        await UniTask.SwitchToThreadPool();
        //日本語ファイルの文字化けを防ぐために文字コードをshift-jisで指定して解凍
        ZipFile.ExtractToDirectory(_path, Directory.GetParent(_path).FullName, Encoding.GetEncoding("shift_jis"));

        //zipファイルの削除
        System.IO.File.Delete(_path);
        await UniTask.SwitchToMainThread();
    }
}
