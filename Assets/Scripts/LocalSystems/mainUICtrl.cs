using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class mainUICtrl : MonoBehaviour
{
    public GameData[] _AllGameDatas { set; get; } = null;

    [SerializeField] Text _debugDatailDatasTxt;

    [SerializeField] Text _titleTxt;
    [SerializeField] Text _editorTxt;
    [SerializeField] Text _devText;
    [SerializeField] Text _setumeiTxt;

    int _nowIndex = 0;
    
    public async UniTask _setDatas(GameData[] g)
    {
        _AllGameDatas = g;
    }

    public async UniTask LoadData()
    {
        if(_AllGameDatas == null)
        {
            return;
        }

        if (_AllGameDatas[_nowIndex] == null)
        {
            _nowIndex = 0;
        }

        string _debugDatailDatas = $"FileName:{_AllGameDatas[_nowIndex].FileName}\r\nGameName:{_AllGameDatas[_nowIndex].GameName}\r\nExeName:{_AllGameDatas[_nowIndex].ExeName}" +
            $"\r\nDevName:{_AllGameDatas[_nowIndex].DevName}";
        _debugDatailDatasTxt.text = _debugDatailDatas;

        _titleTxt.text = _AllGameDatas[_nowIndex].GameName;
        _editorTxt.text = _AllGameDatas[_nowIndex].SoftwareType;
        _devText.text = _AllGameDatas[_nowIndex].DevName;
        _setumeiTxt.text = _AllGameDatas[_nowIndex].Description;
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
