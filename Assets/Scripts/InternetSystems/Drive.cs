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
using System;
using UnityEngine.Windows;

public class Drive
{
    //�h���C�u�T�[�r�X���i�[�ł��A�p�u���b�N����擾�ł���悤��
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
                await DlFile(s, _dirPathJ, ".json");

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
            Debug.Log("�_�E�����[�h�����s��");
            await DlFile(_data.DriveId, new LocalDirPaths()._gameFilePath,"");
        }
        catch (System.Exception e)
        {
            Debug.Log("�Q�[���̃_�E�����[�h�Ɏ��s���܂����B\r�G���[���e�F" + e);
            return false;
        }

        try
        {
            Debug.Log("�t�@�C�����𓀒�");
            await ExtractZIP(new LocalDirPaths()._gameFilePath + "\\" + _data.FileName + ".zip");
        }
        catch (System.Exception e)
        {
            Debug.Log("zip�t�@�C���̉𓀂Ɏ��s���܂����B\r�G���[���e�F" + e);
            return false;
        }

        Debug.Log("�Q�[���̃_�E�����[�h�ɐ���");
        return true;
    }

    public async UniTask<string> UploadGame(string _filePath)
    {
        //API���쐬����Ă��邩���m�F����B
        if (_driveService == null)
        {
            throw new Exception("API���쐬����Ă��܂���");
        }

        //�X���b�h�̐؂�ւ�
        await UniTask.SwitchToThreadPool();
        //�A�b�v���[�h����t�@�C���̃��^�f�[�^���쐬
        var _fileMetaData = new Google.Apis.Drive.v3.Data.File()
        {
            Name = Path.GetFileName(_filePath),
            Parents = new[] { new InternetDatas().GAME_FOLDER_ID }
        };
        Debug.Log("�A�b�v���[�h�t�@�C���̃��^�f�[�^���쐬����");

        FileStream fileStream = new FileStream(_filePath, FileMode.Open);

        var _request = _driveService.Files.Create(_fileMetaData, fileStream, "application/zip");
        //�A�b�v���[�h�����ۂɃh���C�uid�����N�G�X�g���Ă���
        _request.Fields = "id";
        var uploadProgress = _request.Upload();
        fileStream.Close();

        if(uploadProgress.Status != UploadStatus.Completed)
        {
            throw new Exception("�A�b�v���[�h�Ɏ��s���܂����B\r�G���[���e�F" + uploadProgress.Status);
        }

        //�A�b�v���[�h�����t�@�C����ID���擾
        var file = _request.ResponseBody;
        return file.Id;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="_filePath"></param>
    /// <param name="_fileName">�������̓Q�[���t�@�C���̖��O�̂ق�</param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async UniTask<string> UploadImg(string _filePath, string _fileName)
    {
        //API���쐬����Ă��邩���m�F
        if(_driveService == null)
        {
            throw new Exception("API���쐬����Ă��܂���");
        }

        //�X���b�h��؂�ւ�
        await UniTask.SwitchToThreadPool();
        //�A�b�v���[�h����t�@�C���̃��^�f�[�^���쐬
        var _fileMetaData = new Google.Apis.Drive.v3.Data.File()
        {
            Name = _fileName + ".png",
            Parents = new[] {new InternetDatas().IMAGE_FOLDER_ID}
        };
        Debug.Log("�A�b�v���[�h�t�@�C���̃��^�f�[�^���쐬����");

        FileStream fileStream = new FileStream(_filePath, FileMode.Open);

        var _request = _driveService.Files.Create(_fileMetaData, fileStream, "image/png");
        _request.Fields = "id";
        var uploadProgress = _request.Upload();
        if(uploadProgress.Status != UploadStatus.Completed)
        {
            throw new Exception(uploadProgress.Status.ToString());
        }

        var file = _request.ResponseBody;
        return file.Id;
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
    public async UniTask DlFile(string _id, string _path, string _extension)
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
        var fileStream = new FileStream(Path.Combine(_path, (file.Name + _extension)), FileMode.Create, FileAccess.Write);
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
        ZipFile.ExtractToDirectory(_path, new LocalDirPaths()._gameFilePath, Encoding.GetEncoding("shift_jis"));

        //zip�t�@�C���̍폜
        System.IO.File.Delete(_path);
        await UniTask.SwitchToMainThread();
    }


    public async UniTask CreateZIP(string _path)
    {
        await UniTask.SwitchToThreadPool();

        try
        {
            ZipFile.CreateFromDirectory(_path, _path + ".zip", System.IO.Compression.CompressionLevel.Optimal,false,System.Text.Encoding.GetEncoding("shift_jis"));
        }
        catch(Exception e)
        {
            await UniTask.SwitchToMainThread();
            Debug.Log("�t�@�C���̈��k�Ɏ��s���܂���\r�G���[���e�F"�@+ e);
        }


        await UniTask.SwitchToMainThread();
    }
}
