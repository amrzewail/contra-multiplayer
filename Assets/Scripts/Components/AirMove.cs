using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirMove : MonoBehaviour, IMover
{
    [SerializeField] Rigidbody2D rigidbody;
    [SerializeField] float speedLimit;
    [SerializeField] float speed;
    [SerializeField] float acceleration;


    public void Move(Vector2 axis)
    {
        var v = rigidbody.velocity;
        v.x = Mathf.MoveTowards(v.x, v.x + axis.x * speed, acceleration * Time.deltaTime);
        v.x = Mathf.Clamp(v.x, -speedLimit, speedLimit);
        rigidbody.velocity = v;
    }
}
