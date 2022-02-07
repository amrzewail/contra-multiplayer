using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletNormal : NetworkBehaviourOwner, IBullet
{
    [SerializeField] int _index = 0;

    public int index => _index;

    public float speed = 1;

    [SyncVar]
    private Vector3 _direction;

    public override void MyStart()
    {
        Invoke("DestroySelf", 2);
    }

    [Command]
    public void DestroySelf()
    {
        Destroy(this.gameObject);
    }


    public override void MyUpdate()
    {
        transform.position += _direction.normalized * speed * Time.deltaTime;

    }

    public void SetDirection(Vector2 direction)
    {
        _direction = direction;
    }
}
