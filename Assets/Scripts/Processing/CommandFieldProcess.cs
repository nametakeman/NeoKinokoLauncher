using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommandFieldProcess : MonoBehaviour
{
    [SerializeField] InputField _commandInputField;
    [SerializeField] GameObject _modeChange;

    /// <summary>
    /// inputFieldに入力されたコマンドを実行するメソッド
    /// inputFieldのOnEndEditにアタッチされるのでエンターを押した時点で呼び出される。
    /// </summary>
    public void EnteredTx()
    {
        //このメソッドが呼び出された時点の入力されている文字列を取得
        string _sendCommand = _commandInputField.text;

        //空欄だった場合処理を中止
        if (_sendCommand == "") return;

        //テキストを半角スペースで区切り、配列に格納
        string[] _cmds = _sendCommand.Split("");

        if (_cmds[0] == "upload")
        {
            _modeChange.GetComponent<ModeChanger>().SwitchMode("upload");
        }
        else if (_cmds[0] == "exit")
        {
            _modeChange.GetComponent<ModeChanger>().SwitchMode("main");
        }


    }
}
