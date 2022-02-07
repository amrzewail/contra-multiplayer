using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInput
{
    public float GetHorizontal();

    public float GetVertical();

    public bool Jump();

    public bool JumpHold();

    public bool IsRunning();

    public bool IsCrouching();

    public bool Shoot();
}
