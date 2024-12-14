using Cysharp.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Upload;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Drive
{
    //�h���C�u�T�[�r�X���i�[�ł��A�p�u���b�N����擾�ł���悤�ɍ쐬�B�Ȃ���΍��
    public DriveService _driveService {
        get 
        {
            if (_driveService == null)
            {
                _driveService = _createAPI();
            }
            return _driveService;
        }
        private set { _driveService = value; }
    }



    /// <summary>
    /// �C���^�[�l�b�g�ڑ��p��API���쐬�B�ڍׂ�Apis�̃��t�@�����X
    /// </summary>
    public DriveService _createAPI()
    {
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

        return _service;
    }



    public async UniTask <string[]> DriveList(DriveService _ds)
    {
        List<string> _driveList = new List<string>();

        //�t�H���_���̌���
        var _request = _ds.Files.List();
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
    async UniTask DlFile(DriveService _service, string _id, string _path)
    {
        //�X���b�h�����C���X���b�h����؂�ւ�
        await UniTask.SwitchToThreadPool();

        //���^�f�[�^�̎擾
        var file = _service.Files.Get(_id).Execute();
        if(file == null)
        {
            await UniTask.SwitchToMainThread();
            //��O�𓊂���
            throw new System.Exception("failed get meta");
        }

        //�{�t�@�C���̃_�E�����[�h
        var request = _service.Files.Get(_id);
        var fileStream = new FileStream(Path.Combine(_path, file.Name), FileMode.Create, FileAccess.Write);
        request.Download(fileStream);
        fileStream.Close();
        await UniTask.SwitchToMainThread();
    }
}
