using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGrounder
{
    bool IsGrounded();

    Layer GetGroundLayer();

    Collider2D GetGroundCollider(Layer layer);

    bool HasGroundLayer(Layer layer);
}
