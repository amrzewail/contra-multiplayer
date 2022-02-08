using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGrounder
{
    bool IsGrounded();

    string GetGroundLayer();

    bool HasGroundLayer(string layer);
}
