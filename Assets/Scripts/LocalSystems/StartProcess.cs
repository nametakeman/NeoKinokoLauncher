using Cysharp.Threading.Tasks;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�\�t�g�N�����ɂ���Z�b�g�A�b�v�֌W����������N���X
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
    /// �p�u���b�N�ȃ��\�b�h�ŃI�����C�����[�h�ڍs�̔��΂݂̂��s���B(eventtrigger����̎Q�ƂȂ�)
    /// </summary>
    public void StartOnlineMethod()
    {
        OnlineStartProcess().Forget();
    }

    async UniTask StartProMethod()
    {
        DirectorySystems directorySystems = new DirectorySystems();
        //�f�B���N�g������������Ă��邩�̊m�F
        await directorySystems._checkDirectorys();

        //���݂���json�t�@�C�����̃Q�[���f�[�^���擾
        GameData[] _gameDatas =�@await directorySystems._createGameData();

        //�Q�[���ۑ��t�H���_�ɓ����Ă���Q�[���̃t�H���_�����擾
        string[] _localGameDirName = directorySystems._createLocalGameFN();

        //json�t�@�C�����ǉ�����Ă��Ȃ��Q�[�����܂߂��ׂĂ̊m�F������Q�[����GameDatas��z��Ƃ��Ď擾����
        GameData[] _allGameDatas = await directorySystems._createAllGameDatas(_gameDatas, _localGameDirName);

        //�Q�[�����_�E�����[�h����Ă��邩���m�F����
        await directorySystems._checkStatus(_allGameDatas);

        //mainData�I�u�W�F�N�g�ɃQ�[���̃f�[�^��n��
        await mainUICtrl.GetComponent<mainUICtrl>()._setDatas(_allGameDatas);

        //UI�̃��[�h
        await mainUICtrl.GetComponent<mainUICtrl>().LoadData();
    }

    /// <summary>
    /// ���ۂ̃I�����C�����[�h�ڍs�͂������ōs��
    /// </summary>
    async UniTask OnlineStartProcess()
    {
        //Drive��ɂ���json�t�@�C���̃��^�f�[�^���擾����B�i�����Ńl�b�g�Ɍq�����Ă��邩�𔻒f)
        Drive _drive = new Drive();
        DriveService _driveService = _drive._createAPI();


        string[] _strA = await _drive.DriveList(_driveService);

    }
}
