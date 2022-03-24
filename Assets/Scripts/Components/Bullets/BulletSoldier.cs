using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BulletSoldier : NetworkBehaviourOwner, IBullet
{
    [SerializeField] int _index = 0;
    [SerializeField] float destroyAfter = 1f;
    [SerializeField] GameObject _explosion;
    [SerializeField] SFX _sfx;
    public SFX sfx => _sfx;
    public int index => _index;

    public float speed = 1;

    [SyncVar]
    private Vector3 _direction;

    [SyncVar]
    private uint _shooterId;

    public int bullets { get; }

    public float reloadInterval { get; }

    public int fireRate => 10;

    public bool isContinuous => false;

    public override void ServerStart()
    {
        var instance = FindObjectsOfType<NetworkBehaviourOwner>().First(x => x.netId.Equals(_shooterId));

        transform.position += instance.transform.position - Vector3.one * 9000;

        Invoke("CmdDestroySelf", destroyAfter);

        GetComponentInChildren<Damage>().OnHit.AddListener(OnHitCallback);
    }

    public override void ClientStart()
    {
        if (GetComponent<Rigidbody2D>())
        {
            Destroy(GetComponent<Rigidbody2D>());
        }
        GetComponentInChildren<Damage>().OnHit.AddListener(OnHitCallback);
    }

    private void OnHitCallback(IHitbox hitbox)
    {
        if (hitbox.gameObject.GetComponent<MonoBehaviourOwner>().isMine)
        {
            Destroy();
            CmdDestroySelf();
        }
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

    public void Destroy()
    {
        GameObject.Instantiate(_explosion).transform.position = this.transform.position;
        Destroy(this.gameObject);
    }

    public override void ServerUpdate()
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
