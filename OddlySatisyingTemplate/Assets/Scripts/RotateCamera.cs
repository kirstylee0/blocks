using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCamera : MonoBehaviour
{
    [SerializeField] float _currentRotationValue;
    public Spring yRotationSpring;
    void Start()
    {
        _currentRotationValue = transform.eulerAngles.y;
        yRotationSpring.CurrentValue = _currentRotationValue;
        //Debug.Log("Current Value " + yRotationSpring.CurrentValue);
        yRotationSpring.Target = _currentRotationValue;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            _currentRotationValue += 90;
            yRotationSpring.Target = _currentRotationValue;
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            _currentRotationValue -= 90;
            yRotationSpring.Target = _currentRotationValue;
        }

        transform.eulerAngles = new Vector3(transform.eulerAngles.x, yRotationSpring.CurrentValue, transform.eulerAngles.z);
        yRotationSpring.Update();
    }
}
