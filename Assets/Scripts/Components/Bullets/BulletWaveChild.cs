using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BulletWaveChild : MonoBehaviourOwner, IBullet
{
    [SerializeField] GameObject _explosion;
    [SerializeField] SFX _sfx;
    public SFX sfx => _sfx;

    public int index => 0;

    public int bullets => throw new NotImplementedException();

    public float reloadInterval => throw new NotImplementedException();

    public int fireRate => throw new NotImplementedException();

    public bool isContinuous => throw new NotImplementedException();

    private bool _didExplode = false;

    public Action OnExplodeEvent;

    public override void OtherStart()
    {
        GetComponentInChildren<Damage>().OnHit.AddListener(OnHitCallback);
    }

    public override void MyStart()
    {
        //var instance = FindObjectsOfType<NetworkBehaviourOwner>().First(x => x.netId.Equals(_shooterId));

        //transform.position += instance.transform.position - Vector3.one * 9000;

        //Invoke("CmdDestroySelf", destroyAfter);

        GetComponentInChildren<Damage>().OnHit.AddListener(OnHitCallback);
    }


    public override void MyUpdate()
    {
    }

    protected void OnHitCallback(IHitbox hitbox)
    {
        OnExplodeEvent?.Invoke();
        //CmdDestroySelf();
    }


    public void Explode()
    {
        if (_didExplode) return;
        _didExplode = true;
        GameObject.Instantiate(_explosion).transform.position = this.transform.position;
        this.gameObject.SetActive(false);
    }

    public void SetDirection(Vector2 direction)
    {
        throw new NotImplementedException();
    }

    public void SetShooterId(uint id)
    {
        throw new NotImplementedException();
    }
}
