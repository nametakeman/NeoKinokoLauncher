using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingKinoko : MonoBehaviour
{

    [SerializeField] GameObject modeChanger;
    int _counter = 0;
    float _startTime;

    void Update()
    {
        this.transform.Rotate(new Vector3(0, 0, -5) * Time.deltaTime);
    }

    public void pushedKinoko()
    {

        if (_counter == 0)
        {
            _startTime = Time.time;
        }
        else if (_counter == 6 && Time.time <= (_startTime + 5))
        {
            modeChanger.GetComponent<ModeChanger>().SwitchMode("command");
            _counter = 0;
        }
        else if (Time.time >= _startTime + 5)
        {
            _counter = 0;
            _startTime = Time.time;
        }

        _counter++;

    }
}
