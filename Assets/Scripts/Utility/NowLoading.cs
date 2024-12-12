using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NowLoading : MonoBehaviour
{
    Text _loadingTxt;
    int _flameCounter = 0;

    private void Start()
    {
        _loadingTxt = GetComponent<Text>();
        _loadingTxt.text = "Now Loading";
    }

    private void Update()
    {
        _flameCounter++;
        if(_flameCounter % 60 == 0 && _flameCounter != 0)
        {
            _loadingTxt.text = _loadingTxt.text + ".";
        }

        if(_flameCounter >= 240)
        {
            _flameCounter = 0;
            _loadingTxt.text = "Now Loading";
        }
    }



}
