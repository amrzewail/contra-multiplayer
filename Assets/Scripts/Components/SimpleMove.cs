using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMove : MonoBehaviour, IMover
{
    [SerializeField] Rigidbody2D rigidbody;
    [SerializeField] float speed;
    public void Move(Vector2 axis)
    {
        var v = rigidbody.velocity;
        v.x = axis.x * speed;
        rigidbody.velocity = v;
        //_syncer.SetValue(v);
    }
}
