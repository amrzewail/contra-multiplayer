using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnTrigger : MonoBehaviourOwner
{
    [SerializeField] string layer = "Ground";

    public bool serverAuthority = false;
    public bool isTriggering;
    public UnityEvent<Collider2D> OnTriggerEnter;

    private List<Collider2D> _colliders = new List<Collider2D>();

    public void ForceTrigger()
    {
        OnTriggerEnter?.Invoke(null);
    }

    public override void ServerOnTriggerEnter2D(Collider2D collider)
    {
        InternalOnTriggerEnter2D(collider);
    }
    public override void ServerOnTriggerExit2D(Collider2D collider)
    {
        InternalOnTriggerExit2D(collider);
    }
    public override void ClientOnTriggerEnter2D(Collider2D collider)
    {
        if (!serverAuthority) InternalOnTriggerEnter2D(collider);
    }
    public override void ClientOnTriggerExit2D(Collider2D collider)
    {
        if (!serverAuthority) InternalOnTriggerExit2D(collider);
    }

    private void  InternalOnTriggerEnter2D(Collider2D other)
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

    private void InternalOnTriggerExit2D(Collider2D other)
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
