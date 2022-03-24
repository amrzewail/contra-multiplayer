using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BulletNormal : BulletBase, IBullet
{

    [SerializeField] int _index = 0;
    [SerializeField] float destroyAfter = 1f;
    [SerializeField] SFX _sfx;
    public SFX sfx => _sfx;

    public int index => _index;


    public override void OtherStart()
    {
        GetComponentInChildren<Damage>().OnHit.AddListener(OnHitCallback);
    }

    public override void MyStart()
    {
        var instance = FindObjectsOfType<NetworkBehaviourOwner>().First(x => x.netId.Equals(_shooterId));

        transform.position += instance.transform.position - Vector3.one * 9000;

        Invoke("CmdDestroySelf", destroyAfter);

        GetComponentInChildren<Damage>().OnHit.AddListener(OnHitCallback);
    }


    public override void MyUpdate()
    {
        transform.position += _direction.normalized * speed * Time.deltaTime;

    }
}
