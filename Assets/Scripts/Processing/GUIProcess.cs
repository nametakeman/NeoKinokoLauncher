using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
        DlGameProcessing().Forget();
    }

    public void ClickedConnectOnline()
    {
        _startProcess.GetComponent<StartProcess>().StartOnlineMethod();
    }

    public void ClickedImage()
    {
        DlImageProcessing().Forget();
    }

    async UniTask DlImageProcessing()
    {

        int _index = _mainUICtrl.GetComponent<mainUICtrl>()._nowIndex;
        string _filePath = new LocalDirPaths()._imageFolderPath;
        if (File.Exists(_filePath + "\\" + _AllGameDatas[_index].FileName + ".png"))
        {
            return;
        }

        _ModeChanges.GetComponent<ModeChanger>().SwitchMode("loading");
        Drive _drive = new Drive();
        try
        {
            await _drive._createAPI();
        }
        catch (System.Exception e)
        {
            Debug.Log("API�̍쐬�Ɏ��s���܂���\r�G���[���e�F" + e);
            _ModeChanges.GetComponent<ModeChanger>().SwitchMode("main");
            return;
        }

        try
        {
            await  _drive.DlFile(_AllGameDatas[_index].ImageId, _filePath, "");
        }
        catch (System.Exception e) 
        {
            Debug.Log("�摜�t�@�C���̕ۑ��Ɏ��s���܂���\r�G���[���e�F" + e);
            _ModeChanges.GetComponent<ModeChanger>().SwitchMode("main");
            return;
        }

        //UI���ă��[�h
        await _startProcess.GetComponent<StartProcess>().StartProMethod();

        _ModeChanges.GetComponent<ModeChanger>().SwitchMode("main");
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
            Debug.Log("API�̍쐬�Ɏ��s���܂���\r�G���[���e�F" + e);
            _ModeChanges.GetComponent<ModeChanger>().SwitchMode("main");
            return;
        }

        if (!await _drive.DlGame(_AllGameDatas[_mainUICtrl.GetComponent<mainUICtrl>()._nowIndex]))
        {
            _ModeChanges.GetComponent<ModeChanger>().SwitchMode("main");
            return;
        }

        Version version = new Version();
        version._gameName = _AllGameDatas[_mainUICtrl.GetComponent<mainUICtrl>()._nowIndex].FileName;
        version._gameVersion = _AllGameDatas[_mainUICtrl.GetComponent<mainUICtrl>()._nowIndex].Version;

        ManagedVersion managedVersion = new ManagedVersion();
        List<Version> versionList = new List<Version>();
        try
        {
            versionList = new List<Version> ( managedVersion.TextToVersionClass() );
        }
        catch(System.Exception e)
        {
            versionList.Add(version);
        }
        versionList.Add(version);
        managedVersion.ClassToText(versionList.ToArray());

        //UI���ă��[�h
        await _startProcess.GetComponent<StartProcess>().StartProMethod();

        _ModeChanges.GetComponent<ModeChanger>().SwitchMode("main");
    }



    public async UniTask UploadProcessing(string _folderPath, string _gameName, string _exeName, string _devName, string _software, string _description, string _imagePath)
    {
        _ModeChanges.GetComponent<ModeChanger>().SwitchMode("loading");
        string _driveId = "";
        string _imageId = "";

        Drive _drive = new Drive();
        try
        {
            await _drive._createAPI();
        }
        catch (System.Exception e)
        {
            Debug.Log("API�̍쐬�Ɏ��s���܂���\r�G���[���e�F" + e);
            _ModeChanges.GetComponent<ModeChanger>().SwitchMode("main");
            return;
        }

        Debug.Log("�Q�[���̈��k���J�n���܂�");
        try
        {
            await _drive.CreateZIP(_folderPath);
        }
        catch (System.Exception e)
        {
            Debug.Log("�Q�[���̈��k�Ɏ��s���܂���\r�G���[���e�F" + e);
            _ModeChanges.GetComponent<ModeChanger>().SwitchMode("main");
            return;
        }

        Debug.Log("�Q�[���̃A�b�v���[�h���J�n���܂�");
        try
        {
            _driveId = await _drive.UploadGame(_folderPath + ".zip");
        }
        catch(System.Exception e)
        {
            Debug.Log("�t�@�C���̃A�b�v���[�h�Ɏ��s���܂���\r�G���[���e�F" + e);
            _ModeChanges.GetComponent<ModeChanger>().SwitchMode("main");
            return;
        }
        finally
        {
            if(File.Exists(_folderPath + ".zip"))
            {
                File.Delete(_folderPath + ".zip");
            }
        }

        string[] _splitArray = _folderPath.Split(@"\");

        if(_imagePath != "")
        {
            Debug.Log("�ʐ^�t�@�C���̃A�b�v���[�h���J�n���܂�");
            try
            {
                _imageId = await _drive.UploadImg(_imagePath, _splitArray[_splitArray.Length - 1]);
            }
            catch(System.Exception e)
            {
                Debug.Log("�ʐ^�t�@�C���̃A�b�v���[�h�Ɏ��s���܂���\r�G���[���e�F" + e);
                _ModeChanges.GetComponent<ModeChanger>().SwitchMode("main");
                return;
            }
        }


        Debug.Log("�V�[�g�ւ̏������݂��J�n���܂�");
        SS ss = new SS();
        try
        {
            await ss._createAPI();
        }
        catch (System.Exception e)
        {
            Debug.Log("SS��API�쐬�Ɏ��s���܂���\r�G���[���e�F" + e);
            _ModeChanges.GetComponent<ModeChanger>().SwitchMode("main");
            return;
        }


        try
        {
            await ss.writingSS(new string[] { _splitArray[_splitArray.Length - 1],_gameName,_exeName,_devName,_driveId,_imageId,_software,_description});
        }
        catch (System.Exception e)
        {
            Debug.Log("SS�̏������݂Ɏ��s���܂���\r�G���[���e�F" + e);
            _ModeChanges.GetComponent<ModeChanger>().SwitchMode("main");
            return;
        }

        //UI���ă��[�h
        await _startProcess.GetComponent<StartProcess>().StartProMethod();
        _ModeChanges.GetComponent<ModeChanger>().SwitchMode("main");
    }
}
