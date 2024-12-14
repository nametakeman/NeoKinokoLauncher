using Cysharp.Threading.Tasks;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ソフト起動時にするセットアップ関係処理をするクラス
public class StartProcess : MonoBehaviour
{
    [SerializeField] GameObject mainUICtrl;
    // Start is called before the first frame update
    void Start()
    {
        StartProMethod().Forget();
        StartOnlineMethod();
    }



    /// <summary>
    /// パブリックなメソッドでオンラインモード移行の発火のみを行う。(eventtriggerからの参照など)
    /// </summary>
    public void StartOnlineMethod()
    {
        OnlineStartProcess().Forget();
    }

    async UniTask StartProMethod()
    {
        DirectorySystems directorySystems = new DirectorySystems();
        //ディレクトリが生成されているかの確認
        await directorySystems._checkDirectorys();

        //存在するjsonファイル分のゲームデータを取得
        GameData[] _gameDatas =　await directorySystems._createGameData();

        //ゲーム保存フォルダに入っているゲームのフォルダ名を取得
        string[] _localGameDirName = directorySystems._createLocalGameFN();

        //jsonファイルが追加されていないゲームも含めすべての確認しうるゲームのGameDatasを配列として取得する
        GameData[] _allGameDatas = await directorySystems._createAllGameDatas(_gameDatas, _localGameDirName);

        //ゲームがダウンロードされているかを確認する
        await directorySystems._checkStatus(_allGameDatas);

        //mainDataオブジェクトにゲームのデータを渡す
        await mainUICtrl.GetComponent<mainUICtrl>()._setDatas(_allGameDatas);

        //UIのロード
        await mainUICtrl.GetComponent<mainUICtrl>().LoadData();
    }

    /// <summary>
    /// 実際のオンラインモード移行はこっちで行う
    /// </summary>
    async UniTask OnlineStartProcess()
    {
        //Drive上にあるjsonファイルのメタデータを取得する。（ここでネットに繋がっているかを判断)
        Drive _drive = new Drive();
        DriveService _driveService = _drive._createAPI();


        string[] _strA = await _drive.DriveList(_driveService);

    }
}
