using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GroundCheckerBase : MonoBehaviourOwner, IGrounder
{
    [SerializeField] LayerMask layerMask;

    public bool isGrounded;

    private List<Collider2D> _colliders = new List<Collider2D>();
    private int _currentLayer;

    public UnityEvent<int> OnGrounded;

    protected void BaseOnTriggerEnter2D(Collider2D collision)
    {
        if ((layerMask & 1 << collision.gameObject.layer) == 1 << collision.gameObject.layer)
        {
            if (!_colliders.Contains(collision))
            {
                _colliders.Add(collision);
                _currentLayer = collision.gameObject.layer;
                isGrounded = true;
                OnGrounded?.Invoke(_currentLayer);
            }
        }
    }

    protected void BaseOnTriggerExit2D(Collider2D collision)
    {
        if ((layerMask & 1 << collision.gameObject.layer) == 1 << collision.gameObject.layer)
        {
            if (_colliders.Contains(collision))
            {
                _colliders.Remove(collision);
                if (_colliders.Count == 0)
                {
                    isGrounded = false;
                    _currentLayer = 0;
                }
                else
                {
                    _currentLayer = _colliders[_colliders.Count - 1].gameObject.layer;
                }
            }
        }
    }

    protected void BaseFixedUpdate()
    {
        bool didRemove = false;
        for (int i = 0; i < _colliders.Count; i++)
        {
            if (!_colliders[i])
            {
                _colliders.RemoveAt(i);
                didRemove = true;
                if (_colliders.Count == 0)
                {
                    isGrounded = false;
                    _currentLayer = 0;
                }
                break;
            }
        }

        if(didRemove && isGrounded)
        {
            _currentLayer = _colliders[_colliders.Count - 1].gameObject.layer;
        }
    }

    public bool IsGrounded()
    {
        return isGrounded;
    }

    public Layer GetGroundLayer()
    {
        return (Layer)_currentLayer;
    }

    public bool HasGroundLayer(Layer layer)
    {
        for(int i = 0; i < _colliders.Count; i++)
        {
            int n =_colliders[i].gameObject.layer;
            if (n.Equals((int)layer)) return true;
        }
        return false;
    }

    public Collider2D GetGroundCollider(Layer layer)
    {
        for (int i = 0; i < _colliders.Count; i++)
        {
            int n = _colliders[i].gameObject.layer;
            if (n.Equals((int)layer)) return _colliders[i];
        }
        return null;
    }
}
