using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleJump : MonoBehaviour, IJumper
{
    public Rigidbody2D rigidbody;
    public float force = 10;

    public void Jump()
    {
        var v = rigidbody.velocity;
        v.y += force;
        rigidbody.velocity = v;
    }
}
