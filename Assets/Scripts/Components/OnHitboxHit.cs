using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnHitboxHit : MonoBehaviour
{
    public UnityEvent OnHit;
    public Hitbox hitbox;

    internal void Update()
    {
        if (hitbox.IsHit())
        {
            OnHit?.Invoke();
        }
    }
}
