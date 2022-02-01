using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundChecker : MonoBehaviour, IGrounder
{
    [SerializeField] string groundLayer = "Ground";

    public bool isGrounded;

    private List<Collider2D> _colliders = new List<Collider2D>();

    internal void OnTriggerEnter2D(Collider2D other)
    {
        if (LayerMask.NameToLayer(groundLayer).Equals(other.gameObject.layer))
        {
            if (!_colliders.Contains(other))
            {
                _colliders.Add(other);
                isGrounded = true;
            }
        }
    }

    internal void OnTriggerExit2D(Collider2D other)
    {
        if (LayerMask.NameToLayer(groundLayer).Equals(other.gameObject.layer))
        {
            if (_colliders.Contains(other))
            {
                _colliders.Remove(other);
                if (_colliders.Count == 0) isGrounded = false;
            }
        }
    }

    internal void FixedUpdate()
    {
        for (int i = 0; i < _colliders.Count; i++)
        {
            if (!_colliders[i])
            {
                _colliders.RemoveAt(i);
                if (_colliders.Count == 0) isGrounded = false;
                break;
            }
        }
    }

    public bool IsGrounded()
    {
        return isGrounded;
    }
}
