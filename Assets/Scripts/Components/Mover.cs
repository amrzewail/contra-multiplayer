using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Mover : MonoBehaviour
{
    public Vector2 velocity;

    private Rigidbody2D _rb;
    private SpriteRenderer _renderer;

    private void Start()
    {
        _renderer = GetComponentInChildren<SpriteRenderer>();
        _rb = GetComponentInChildren<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        var v = _rb.velocity;
        if (velocity.x != 0) v.x = velocity.x;
        if (velocity.y != 0) v.y = velocity.y;
        _rb.velocity = v;

        if (_renderer)
        {
            if (velocity.x > 0) _renderer.transform.localScale = new Vector3(1, 1, 1);
            else if (velocity.x < 0) _renderer.transform.localScale = new Vector3(-1, 1, 1);
        }
    }
}
