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
    //�h���C�u�T�[�r�X���i�[�ł��A�p�u���b�N����擾�ł���悤�ɍ쐬�B�Ȃ���΍��
    public DriveService _driveService { get; private set; }



    /// <summary>
    /// �C���^�[�l�b�g�ڑ��p��API���쐬�B�ڍׂ�Apis�̃��t�@�����X
    /// </summary>
    public async UniTask _createAPI()
    {
        if(_driveService != null)
        {
            return;
        }
        //������ւ�̓}�W�Ő������Y�C��������̃��t�@�����X�ǂ�ł���[�[�[

        //drive�T�[�r�X�̔F�؏����擾����
        GoogleCredential credential;
        using (var stream = new FileStream(new InternetDatas().JSON_FILE_PATH, FileMode.Open, FileAccess.Read))
        {
            credential = GoogleCredential.FromStream(stream).CreateScoped(DriveService.ScopeConstants.Drive);
        }

        //DriveApi�̃T�[�r�X���쐬
        DriveService _service = new DriveService( new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = "DriveService"
        } );

        _driveService =  _service;
    }

    /// <summary>
    /// ���ׂĂ�json�t�@�C����DL����B
    /// </summary>
    /// <returns>false=���s</returns>
    public async UniTask<bool> DlAllJson()
    {
        if (_driveService == null)
        {
            Debug.Log("API���쐬����Ă��܂���<DlAllJson>");
            return false;
        }
        //�h���C�u�ɕۑ�����Ă���json�f�[�^��id�̔z��Ŏ擾���Ă���B�����������ȏꍇ�̓l�b�g�ɐڑ�����Ă��Ȃ����̂Ƃ���B
        string[] _strA = null;
        try
        {
            _strA = await DriveList();
        }
        catch (System.Exception e)
        {
            Debug.Log("�C���^�[�l�b�g�ɐڑ�����Ă��܂���\r�G���[���e:" + e);
            return false;
        }

        string _dirPathJ = new LocalDirPaths()._jsonFolderPath;
        int _counter = 0;
        foreach (string s in _strA)
        {
            Debug.Log($"json�t�@�C�����_�E�����[�h��({_counter}/{_strA.Length})");
            try
            {
                await DlFile(s, _dirPathJ);

            }
            catch (System.Exception e)
            {
                Debug.Log("�_�E�����[�h�Ɏ��s���܂����B\r�G���[���e�F" + e);
                return false;
            }
            _counter++;
        }

        return true;
    }

    /// <summary>
    /// �Q�[�����_�E�����[�h���邽�߂̊֐�
    /// </summary>
    /// <returns></returns>
    public async UniTask<bool> DlGame(GameData _data)
    {
        //API���쐬����Ă��邩�̊m�F
        if (_driveService == null)
        {
            Debug.Log("API���쐬����Ă��܂���<DlAllJson>");
            return false;
        }

        if(_data.DriveId == "")
        {
            Debug.Log("DriveId���������Ă��܂���");
            return false;
        }

        try
        {
            await DlFile(_data.DriveId, new LocalDirPaths()._gameFilePath);
        }
        catch (System.Exception e)
        {
            Debug.Log("�Q�[���̃_�E�����[�h�Ɏ��s���܂����B\r�G���[���e�F" + e);
            return false;
        }

        try
        {
            await ExtractZIP(new LocalDirPaths()._gameFilePath + "\\" + _data.FileName + ".zip");
        }
        catch (System.Exception e)
        {
            Debug.Log("zip�t�@�C���̉𓀂Ɏ��s���܂����B\r�G���[���e�F" + e);
        }

        Debug.Log("�Q�[���̃_�E�����[�h�ɐ���");
        return true;
    }



    async UniTask <string[]> DriveList()
    {
        List<string> _driveList = new List<string>();

        //�t�H���_���̌���
        var _request = _driveService.Files.List();
        _request.Q = "'" + new InternetDatas().JSON_FOLDER_ID + "' in parents";
        //�����Ŏ擾����t�B�[���h�����Ă��ł���Bid�Ƃ�name�Ƃ�
        _request.Fields = "nextPageToken, files(id,name)";
        var files = new List<Google.Apis.Drive.v3.Data.File>();

        do
        {
            var result = _request.Execute();
            files.AddRange(result.Files);
            _request.PageToken = result.NextPageToken;
        } while (!string.IsNullOrEmpty(_request.PageToken));

        

        //���ʂ��o�͂���
        foreach (var file in files)
        {
            _driveList.Add(file.Id);
        }
        return _driveList.ToArray();
    }

    /// <summary>
    /// �w�肳�ꂽID�̃t�@�C�����_�E�����[�h����
    /// </summary>
    /// <param name="_id">DriveID</param>
    /// <param name="_path">�ۑ���̃p�X</param>
    /// <returns></returns>
    async UniTask DlFile(string _id, string _path)
    {
        //�X���b�h�����C���X���b�h����؂�ւ�
        await UniTask.SwitchToThreadPool();

        //���^�f�[�^�̎擾
        var file = _driveService.Files.Get(_id).Execute();
        if(file == null)
        {
            await UniTask.SwitchToMainThread();
            //��O�𓊂���
            throw new System.Exception("failed get meta");
        }

        //�{�t�@�C���̃_�E�����[�h
        var request = _driveService.Files.Get(_id);
        var fileStream = new FileStream(Path.Combine(_path, file.Name), FileMode.Create, FileAccess.Write);
        request.Download(fileStream);
        fileStream.Close();
        await UniTask.SwitchToMainThread();
    }

    /// <summary>
    /// �w�肳�ꂽ�p�X�̃t�@�C�����𓀂���A����zip�t�@�C���͍폜�����
    /// </summary>
    /// <param name="_path"></param>
    /// <returns></returns>
    async UniTask ExtractZIP(string _path)
    {
        await UniTask.SwitchToThreadPool();
        //���{��t�@�C���̕���������h�����߂ɕ����R�[�h��shift-jis�Ŏw�肵�ĉ�
        ZipFile.ExtractToDirectory(_path, Directory.GetParent(_path).FullName, Encoding.GetEncoding("shift_jis"));

        //zip�t�@�C���̍폜
        System.IO.File.Delete(_path);
        await UniTask.SwitchToMainThread();
    }
}
