using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingKinoko : MonoBehaviour
{

    void Update()
    {
        this.transform.Rotate(new Vector3(0, 0, -5) * Time.deltaTime);
        if(this.transform.rotation.z <= -360)
        {
        }
    }
}
