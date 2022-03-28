using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BulletSpin : BulletBase, IBullet
{

    [SerializeField] int _index = 0;
    [SerializeField] float destroyAfter = 1f;
    [SerializeField] float radius = 1;
    [SerializeField] float spinSpeed = 1;
    [SerializeField] SFX _sfx;
    public SFX sfx => _sfx;
    public int index => _index;

    private Vector3 _straightPosition;

    private float _time = 0;


    public override void OtherStart()
    {
        //GetComponentInChildren<Damage>().OnHit.AddListener(OnHitCallback);
    }

    public override void Start()
    {
        base.Start();

        var instance = FindObjectsOfType<NetworkBehaviourOwner>().First(x => x.netId.Equals(_shooterId));

        transform.position += instance.transform.position - Vector3.one * 9000;
        _straightPosition = transform.position;
        _time = 0;
        Invoke("CmdDestroySelf", destroyAfter);

        GetComponentInChildren<Damage>().OnHit.AddListener(OnHitCallback);
    }


    public override void Update()
    {
        base.Update();

        _direction.Normalize();
        _straightPosition += _direction * speed * Time.deltaTime;

        float y = Mathf.Sin(_time * spinSpeed) * radius;
        float x = (_direction.x == 0? 1 : -Sign(_direction.x)) * Mathf.Cos(_time * spinSpeed) * radius;

        transform.position = _straightPosition + new Vector3(Sign(_direction.x) * radius + x, y, 0);

        _time += Time.deltaTime;

    }

    private int Sign(float x)
    {
        if (x > 0) return 1;
        else if (x < 0) return -1;
        else return 0;
    }
}
