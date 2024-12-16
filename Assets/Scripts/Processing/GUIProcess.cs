using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIProcess : MonoBehaviour
{
    public GameData[] _AllGameDatas { private set; get; } = null;
    

    [SerializeField] GameObject _mainUICtrl;
    [SerializeField] GameObject _startProcess;
    [SerializeField] GameObject _ModeChanges;

    public void SetGameData(GameData[] g)
    {
        _AllGameDatas = g;
    }

    public void ClickedStart()
    {
        //現在のIndexを取得
        int _index = _mainUICtrl.GetComponent<mainUICtrl>()._nowIndex;

        //DirectorySystemsからゲームをスタートさせる
        DirectorySystems _directorySystems = new DirectorySystems();
        try
        {
            _directorySystems._startGame(_AllGameDatas[_index]);
        }
        catch(System.Exception e)
        {
            Debug.Log("ゲームをスタートできませんでした。\rエラー内容：" + e);
        }
    }

    public void ClickedDlGame()
    {
        DlGameProcessing().Forget();
    }

    public void ClickedConnectOnline()
    {
        _startProcess.GetComponent<StartProcess>().StartOnlineMethod();
    }

    async UniTask DlGameProcessing()
    {
        _ModeChanges.GetComponent<ModeChanger>().SwitchMode("loading");

        Drive _drive = new Drive();
        try
        {
            await _drive._createAPI();
        }
        catch(System.Exception e)
        {
            Debug.Log("APIの作成に失敗しました\rエラー内容：" + e);
            _ModeChanges.GetComponent<ModeChanger>().SwitchMode("main");
            return;
        }

        if (!await _drive.DlGame(_AllGameDatas[_mainUICtrl.GetComponent<mainUICtrl>()._nowIndex]))
        {
            _ModeChanges.GetComponent<ModeChanger>().SwitchMode("main");
            return;
        }

            //UIを再ロード
            await _startProcess.GetComponent<StartProcess>().StartProMethod();
        _ModeChanges.GetComponent<ModeChanger>().SwitchMode("main");
    }


}
