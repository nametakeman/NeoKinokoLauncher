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
    /// �C���^�[�l�b�g�ڑ��p��API���쐬�B�ڍׂ�Apis�̃��t�@�����X
    /// </summary>
    //�����̓I�t���C���ł�������..�̂��B�B�H
    public async UniTask<DriveService> _createAPI()
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
