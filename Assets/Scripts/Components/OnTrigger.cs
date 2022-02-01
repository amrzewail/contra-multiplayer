using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnTrigger : MonoBehaviour
{
    [SerializeField] string layer = "Ground";

    public bool isTriggering;
    public UnityEvent<Collider2D> OnTriggerEnter;

    private List<Collider2D> _colliders = new List<Collider2D>();

    internal void OnTriggerEnter2D(Collider2D other)
    {
        if (LayerMask.NameToLayer(layer).Equals(other.gameObject.layer))
        {
            if (!_colliders.Contains(other))
            {
                _colliders.Add(other);
                isTriggering = true;
                OnTriggerEnter?.Invoke(other);
            }
        }
    }

    internal void OnTriggerExit2D(Collider2D other)
    {
        if (LayerMask.NameToLayer(layer).Equals(other.gameObject.layer))
        {
            if (_colliders.Contains(other))
            {
                _colliders.Remove(other);
                if(_colliders.Count == 0) isTriggering = false;
            }
        }
    }
}
