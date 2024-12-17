using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class mainUICtrl : MonoBehaviour
{
    public GameData[] _AllGameDatas { private set; get; } = null;

    [SerializeField] Text _titleTxt;
    [SerializeField] Text _editorTxt;
    [SerializeField] Text _devText;
    [SerializeField] Text _setumeiTxt;
    [SerializeField] GameObject _StartTxt;
    [SerializeField] GameObject _DlTxt;
    [SerializeField] Image _gameImage;
    [SerializeField] Sprite _noimage;

    public int _nowIndex { get; private set; } = 0;
    
    public async UniTask _setDatas(GameData[] g)
    {
        _AllGameDatas = g;
    }

    public async UniTask LoadData()
    {
        if(_AllGameDatas == null || _AllGameDatas.Length == 0)
        {
            return;
        }


        if (_AllGameDatas[_nowIndex] == null)
        {
            _nowIndex = 0;
        }

        string _debugDatailDatas = $"FileName:{_AllGameDatas[_nowIndex].FileName}\r\nGameName:{_AllGameDatas[_nowIndex].GameName}\r\nExeName:{_AllGameDatas[_nowIndex].ExeName}" +
            $"\r\nDevName:{_AllGameDatas[_nowIndex].DevName}";

        _titleTxt.text = _AllGameDatas[_nowIndex].GameName;
        _editorTxt.text = _AllGameDatas[_nowIndex].SoftwareType;
        _devText.text = _AllGameDatas[_nowIndex].DevName;
        _setumeiTxt.text = _AllGameDatas[_nowIndex].Description;

        //画像ファイルパス
        string _filePath = new LocalDirPaths()._imageFolderPath + @"\" + _AllGameDatas[_nowIndex].FileName + ".png";
        //画像データをロードする
        if (File.Exists(_filePath))
        {
            //画像データをバイト配列として読み込む
            byte[] _fileData = File.ReadAllBytes(_filePath);
            //空のテクスチャを作成する
            Texture2D _texture = new Texture2D(0, 0);
            //テクスチャにファイルデータをロードする
            _texture.LoadImage(_fileData);
            //スプライトデータに変換する
            Sprite _sprite = Sprite.Create(_texture, new Rect(0f, 0f, _texture.width, _texture.height),
                new Vector2(0.5f, 0.5f), 100f);
            _gameImage.sprite = _sprite;
        }
        else
        {
            _gameImage.sprite = _noimage;
        }


        if (_AllGameDatas[_nowIndex].Status == "NotInstall")
        {
            _StartTxt.SetActive(false);
            _DlTxt.SetActive(true);
        }
        else if (_AllGameDatas[_nowIndex].Status == "Local" || _AllGameDatas[_nowIndex].Status == "Online")
        {
            _StartTxt.SetActive(true);
            _DlTxt.SetActive(false);
        }
    }

    public void PlusIndex()
    {
        _nowIndex++;
        if (_nowIndex == _AllGameDatas.Count())
        {
            _nowIndex = 0;
        }
        LoadData();
    }

    public void MinusIndex()
    {
        _nowIndex--;
        if(_nowIndex == -1)
        {
            _nowIndex = _AllGameDatas.Count() - 1;
        }
        LoadData();
    }

    
}
