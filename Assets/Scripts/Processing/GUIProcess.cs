using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIProcess : MonoBehaviour
{
    public GameData[] _AllGameDatas { private set; get; } = null;
    

    [SerializeField] GameObject _mainUICtrl;
    [SerializeField] GameObject _startProcess;

    public void SetGameData(GameData[] g)
    {
        _AllGameDatas = g;
    }

    public void ClickedStart()
    {
        //���݂�Index���擾
        int _index = _mainUICtrl.GetComponent<mainUICtrl>()._nowIndex;

        //DirectorySystems����Q�[�����X�^�[�g������
        DirectorySystems _directorySystems = new DirectorySystems();
        try
        {
            _directorySystems._startGame(_AllGameDatas[_index]);
        }
        catch(System.Exception e)
        {
            Debug.Log("�Q�[�����X�^�[�g�ł��܂���ł����B\r�G���[���e�F" + e);
        }
    }

    public void ClickedDlGame()
    {
        Debug.Log("kousio");
        DlGameProcessing().Forget();
    }

    public void ClickedConnectOnline()
    {
        _startProcess.GetComponent<StartProcess>().StartOnlineMethod();
    }

    async UniTask DlGameProcessing()
    {
        Drive _drive = new Drive();
        try
        {
            await _drive._createAPI();
        }
        catch(System.Exception e)
        {
            Debug.Log("API�̍쐬�Ɏ��s���܂���\r�G���[���e�F" + e);
            return;
        }

        if (!await _drive.DlGame(_AllGameDatas[_mainUICtrl.GetComponent<mainUICtrl>()._nowIndex])) return;

        //UI���ă��[�h
        await _startProcess.GetComponent<StartProcess>().StartProMethod();
    }
}
