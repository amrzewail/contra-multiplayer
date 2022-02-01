using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class JumpOnDamage : MonoBehaviour
{
    [SerializeField] bool damagePlayer = true;

    private Vector2 _lastPosition;
    private Vector2 _positionOffset;

    public UnityEvent OnHit;

    internal void Update()
    {
        
    }

    internal void FixedUpdate()
    {
        _positionOffset = (Vector2)transform.position - _lastPosition;
        _lastPosition = transform.position;
    }

    internal void OnTriggerEnter2D(Collider2D collision)
    {
        IHitbox hit;
        if ((hit = collision.GetComponent<IHitbox>()) != null)
        {
            if ((hit.isPlayer && damagePlayer) || (!hit.isPlayer && !damagePlayer))
            {
                if (_positionOffset.y < -0.1f && !hit.isInvincible)
                {
                    hit.Hit();
                    OnHit?.Invoke();
                }
            }
        }
    }
}
