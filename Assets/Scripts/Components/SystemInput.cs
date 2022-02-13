using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;

using static UnityEngine.InputSystem.InputAction;

public class SystemInput : MonoBehaviourOwner, IInput
{
    private float _horizontal;
    private float _vertical;

    private bool _action1;
    private bool _action2;
    private bool _action2Hold;

    public override void MyLateUpdate()
    {
        _action1 = false;
        _action2 = false;
    }

    public void HorizontalCallback(CallbackContext c)
    {
        float value = c.ReadValue<float>();
        _horizontal = value > 0.1f ? 1 : value < -0.1f? -1 : 0;
    }

    public void VerticalCallback(CallbackContext c)
    {
        float value = c.ReadValue<float>();
        _vertical = value > 0.1f ? 1 : value < -0.1f ? -1 : 0;
    }

    public void Action1Callback(CallbackContext c)
    {
        if (c.performed)
        {
            _action1 = true;
        }
    }
    public void Action2Callback(CallbackContext c)
    {
        if (c.performed)
        {
            _action2 = true;
            _action2Hold = true;
        }
        else
        {
            _action2Hold = false;
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
        return _action1;
    }

    public bool ShootDown()
    {
        return _action2;
    }

    public bool ShootHold()
    {
        return _action2Hold;
    }
}
