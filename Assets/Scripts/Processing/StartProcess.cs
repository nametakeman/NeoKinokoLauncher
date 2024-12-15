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
    [SerializeField] GameObject guiProcess;
    [SerializeField] GameObject modeChanger;

    [SerializeField] GameObject OfflineMark;
    // Start is called before the first frame update
    void Start()
    {
        StartProMethod().Forget();
    }



    /// <summary>
    /// パブリックなメソッドでオンラインモード移行の発火のみを行う。(eventtriggerからの参照など)
    /// </summary>
    public void StartOnlineMethod()
    {
        OnlineStartProcess().Forget();
    }

    public async UniTask StartProMethod()
    {
        modeChanger.GetComponent<ModeChanger>().SwitchMode("loading");

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

        //各オブジェクトにゲームのデータを渡す
        await mainUICtrl.GetComponent<mainUICtrl>()._setDatas(_allGameDatas);
        guiProcess.GetComponent<GUIProcess>().SetGameData(_allGameDatas);

        //UIのロード
        await mainUICtrl.GetComponent<mainUICtrl>().LoadData();

        modeChanger.GetComponent<ModeChanger>().SwitchMode("main");
    }

    /// <summary>
    /// 実際のオンラインモード移行はこっちで行う
    /// </summary>
    async UniTask OnlineStartProcess()
    {
        modeChanger.GetComponent<ModeChanger>().SwitchMode("loading");
        //Drive上にあるjsonファイルのメタデータを取得する。（ここでネットに繋がっているかを判断)
        Drive _drive = new Drive();

        try 
        {
            await _drive._createAPI();
        }
        catch(System.Exception e)
        {
            Debug.Log("APIの作成に失敗しました\rエラー内容：" + e);
            OfflineMark.SetActive(true);
            modeChanger.GetComponent<ModeChanger>().SwitchMode("main");
            return;
        }

        //jsonファイルのDL
        if (!await _drive.DlAllJson())
        {
            OfflineMark.SetActive(true);
            modeChanger.GetComponent<ModeChanger>().SwitchMode("main");
            return;
        }

            Debug.Log("インターネットの接続に成功しました。");

        //UIのデータをもう一回やり直し
        await StartProMethod();

        OfflineMark.SetActive(false);
        modeChanger.GetComponent<ModeChanger>().SwitchMode("main");
    }
}
