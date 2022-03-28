using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class BulletBase : NetworkBehaviourOwner
{
    [SerializeField] GameObject _explosion;
    [SerializeField] int _bullets;
    [SerializeField] float _reloadInterval;
    [SerializeField] bool _isContinuous;
    [SerializeField] int _fireRate = 10;

    public float speed = 1;

    [SyncVar]
    protected Vector3 _direction;

    [SyncVar]
    protected uint _shooterId;

    private bool _didExplode = false;

    public int bullets { get => _bullets; }

    public float reloadInterval { get => _reloadInterval; }

    public bool isContinuous => _isContinuous;

    public int fireRate => _fireRate;

    protected void OnHitCallback(IHitbox hitbox)
    {
        Explode();
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

        Debug.Log("Rpc Explode bullet");

        if (!_didExplode) Explode();
    }


    protected virtual void Explode()
    {
        _didExplode = true;
        GameObject.Instantiate(_explosion).transform.position = this.transform.position;
        Destroy(this.gameObject);

        Debug.Log("Explode bullet");

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
