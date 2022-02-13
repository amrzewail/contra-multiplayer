using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Damage : MonoBehaviourOwner
{
    public DamageType damageType = DamageType.Enemy;
    public UnityEvent<IHitbox> OnHit;


    public override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        if (!enabled) return;
        IHitbox hit;
        if ((hit = collision.GetComponent<IHitbox>()) != null)
        {
            if (hit.Hit(damageType))
            {
                OnHit?.Invoke(hit);
            }
        }
    }
}
