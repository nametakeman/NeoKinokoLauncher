using Cysharp.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Upload;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Drive : MonoBehaviour
{
    /// <summary>
    /// インターネット接続用のAPIを作成。詳細はApisのリファレンス
    /// </summary>
    //ここはオフラインでもいける..のか。。？
    public async UniTask<DriveService> _createAPI()
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
