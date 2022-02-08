using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomJump : MonoBehaviour, IJumper
{
    public Rigidbody2D rigidbody;
    public float minForce = 5;
    public float maxForce = 10;

    public void Jump()
    {
        var v = rigidbody.velocity;
        v.y += UnityEngine.Random.Range(minForce, maxForce);
        rigidbody.velocity = v;
    }
}
