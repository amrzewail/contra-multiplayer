using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Damage : MonoBehaviourOwner
{
    [SerializeField] bool damagePlayer = true;
    public bool generalDamage = false;
    public UnityEvent<IHitbox> OnHit;

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        if (!enabled) return;
        IHitbox hit;
        if ((hit = collision.GetComponent<IHitbox>()) != null)
        {
            if (generalDamage || (hit.isPlayer && damagePlayer) || (!hit.isPlayer && !damagePlayer))
            {
                if (generalDamage || damagePlayer || (!damagePlayer && isMine))
                {
                    if (hit.Hit())
                    {
                        OnHit?.Invoke(hit);
                    }
                }
            }
        }
    }
}
