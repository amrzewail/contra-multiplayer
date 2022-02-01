using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Damage : MonoBehaviour
{
    [SerializeField] bool damagePlayer = true;
    public bool generalDamage = false;
    public UnityEvent OnHit;

    internal void OnTriggerEnter2D(Collider2D collision)
    {
        if (!enabled) return;
        IHitbox hit;
        if ((hit = collision.GetComponent<IHitbox>()) != null)
        {
            if (generalDamage || (hit.isPlayer && damagePlayer) || (!hit.isPlayer && !damagePlayer))
            {
                hit.Hit();
                OnHit?.Invoke();
            }
        }
    }
}
