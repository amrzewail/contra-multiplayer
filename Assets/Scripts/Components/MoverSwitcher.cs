using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoverSwitcher : MonoBehaviour
{
    public Mover mover;

    public bool switchOnEnemy = true;

    internal void OnTriggerEnter2D(Collider2D collision)
    {
        int layer = collision.gameObject.layer;
        if (LayerMask.NameToLayer("Ground").Equals(layer) || (switchOnEnemy && LayerMask.NameToLayer("Enemy").Equals(layer)))
        {
            if (Mathf.Abs(mover.GetComponent<Rigidbody2D>().velocity.y) < 0.1f)
            {
                var v = mover.velocity;
                Vector2 offset = GetComponent<Collider2D>().offset;
                if (offset.x > 0)
                {
                    v.x = -Mathf.Abs(v.x);
                }
                else
                {
                    v.x = Mathf.Abs(v.x);
                }
                mover.velocity = v;
            }
        }
    }
}
