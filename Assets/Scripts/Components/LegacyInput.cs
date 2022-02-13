using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegacyInput : MonoBehaviourOwner, IInput
{
    private float _horizontal;
    private float _vertical;

    public override void MyUpdate()
    {
        _horizontal = 0;
        _vertical = 0;

        if (Input.GetKey(KeyCode.RightArrow))
        {
            _horizontal += 1;
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            _horizontal -= 1;
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            _vertical += 1;
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            _vertical -= 1;
        }

    }

    public float GetHorizontal()
    {
        return _horizontal;
    }
    public float GetVertical()
    {
        return _vertical;
    }

    public bool Jump()
    {
        return Input.GetKeyDown(KeyCode.X);
    }


    public bool ShootDown()
    {
        return Input.GetKeyDown(KeyCode.Z);
    }

    public bool ShootHold()
    {
        return Input.GetKey(KeyCode.Z);
    }
}
