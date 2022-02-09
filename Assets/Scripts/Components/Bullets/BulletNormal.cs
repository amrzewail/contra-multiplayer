using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BulletNormal : NetworkBehaviourOwner, IBullet
{
    [SerializeField] int _index = 0;

    public int index => _index;

    public float speed = 1;

    [SyncVar]
    private Vector3 _direction;

    [SyncVar]
    private uint _shooterId;

    public override void OtherStart()
    {
        GetComponentInChildren<Damage>().OnHit.AddListener(OnHitCallback);
    }

    public override void MyStart()
    {
        var instance = FindObjectsOfType<NetworkBehaviourOwner>().First(x => x.netId.Equals(_shooterId));

        transform.position += instance.transform.position;

        Invoke("CmdDestroySelf", 1);

        GetComponentInChildren<Damage>().OnHit.AddListener(OnHitCallback);
    }

    private void OnHitCallback()
    {
        Destroy(this.gameObject);
        CmdDestroySelf();
    }

    [Command(requiresAuthority = false)]
    public void CmdDestroySelf()
    {
        RpcDestroySelf();
    }

    [ClientRpc]
    public void RpcDestroySelf()
    {
        if (!this || !this.gameObject) return;
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

    public void SetShooterId(uint id)
    {
        _shooterId = id;
    }
}
