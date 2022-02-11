using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BulletNormal : NetworkBehaviourOwner, IBullet
{
    [SerializeField] int _index = 0;
    [SerializeField] float destroyAfter = 1f;

    [SerializeField] GameObject _explosion;

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

        Invoke("CmdDestroySelf", destroyAfter);

        GetComponentInChildren<Damage>().OnHit.AddListener(OnHitCallback);
    }

    private void OnHitCallback(IHitbox hitbox)
    {
        Destroy();
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
        Destroy();
    }


    public override void MyUpdate()
    {
        transform.position += _direction.normalized * speed * Time.deltaTime;

    }

    private void Destroy()
    {
        GameObject.Instantiate(_explosion).transform.position = this.transform.position;
        Destroy(this.gameObject);
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
