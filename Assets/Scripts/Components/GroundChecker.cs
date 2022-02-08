using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundChecker : MonoBehaviourOwner, IGrounder
{
    [SerializeField] LayerMask layerMask;

    public bool isGrounded;

    private List<Collider2D> _colliders = new List<Collider2D>();
    private string _currentLayer;

    public override void MyOnTriggerEnter2D(Collider2D collision)
    {
        if ((layerMask & 1 << collision.gameObject.layer) == 1 << collision.gameObject.layer)
        {
            if (!_colliders.Contains(collision))
            {
                _colliders.Add(collision);
                _currentLayer = LayerMask.LayerToName(collision.gameObject.layer);
                isGrounded = true;
            }
        }
    }

    public override void MyOnTriggerExit2D(Collider2D collision)
    {
        if ((layerMask & 1 << collision.gameObject.layer) == 1 << collision.gameObject.layer)
        {
            if (_colliders.Contains(collision))
            {
                _colliders.Remove(collision);
                if (_colliders.Count == 0)
                {
                    isGrounded = false;
                    _currentLayer = null;
                }
                else
                {
                    _currentLayer = LayerMask.LayerToName(_colliders[_colliders.Count - 1].gameObject.layer);
                }
            }
        }
    }

    public override void MyFixedUpdate()
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
                    _currentLayer = null;
                }
                break;
            }
        }

        if(didRemove && isGrounded)
        {
            _currentLayer = LayerMask.LayerToName(_colliders[_colliders.Count - 1].gameObject.layer);
        }
    }

    public bool IsGrounded()
    {
        return isGrounded;
    }

    public string GetGroundLayer()
    {
        return _currentLayer;
    }

    public bool HasGroundLayer(string layer)
    {
        for(int i = 0; i < _colliders.Count; i++)
        {
            string n = LayerMask.LayerToName(_colliders[i].gameObject.layer);
            if (n.Equals(layer)) return true;
        }
        return false;
    }
}
