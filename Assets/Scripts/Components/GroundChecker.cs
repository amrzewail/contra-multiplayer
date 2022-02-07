using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundChecker : MonoBehaviourOwner, IGrounder
{
    [SerializeField] string groundLayer = "Ground";

    public bool isGrounded;

    private List<Collider2D> _colliders = new List<Collider2D>();

    public override void MyOnTriggerEnter2D(Collider2D other)
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

    public override void MyOnTriggerExit2D(Collider2D other)
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

    public override void MyFixedUpdate()
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
