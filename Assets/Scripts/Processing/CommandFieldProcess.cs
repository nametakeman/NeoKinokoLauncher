using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommandFieldProcess : MonoBehaviour
{
    [SerializeField] InputField _commandInputField;
    [SerializeField] GameObject _modeChange;

    /// <summary>
    /// inputField�ɓ��͂��ꂽ�R�}���h�����s���郁�\�b�h
    /// inputField��OnEndEdit�ɃA�^�b�`�����̂ŃG���^�[�����������_�ŌĂяo�����B
    /// </summary>
    public void EnteredTx()
    {
        //���̃��\�b�h���Ăяo���ꂽ���_�̓��͂���Ă��镶������擾
        string _sendCommand = _commandInputField.text;

        //�󗓂������ꍇ�����𒆎~
        if (_sendCommand == "") return;

        //�e�L�X�g�𔼊p�X�y�[�X�ŋ�؂�A�z��Ɋi�[
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
