using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BulletWave : BulletBase, IBullet
{

    [SerializeField] int _index = 0;
    [SerializeField] float destroyAfter = 1f;
    [SerializeField] SFX _sfx;
    public SFX sfx => _sfx;
    public int index => _index;

    private IBullet[] bullets;


    public override void Start()
    {
        base.Start();

        var instance = FindObjectsOfType<NetworkBehaviourOwner>().First(x => x.netId.Equals(_shooterId));

        transform.position += instance.transform.position - Vector3.one * 9000;
        Invoke("CmdDestroySelf", destroyAfter);

        bullets = new IBullet[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            int index = i;
            bullets[i] = transform.GetChild(i).GetComponent<IBullet>();
            ((BulletWaveChild)bullets[i]).OnExplodeEvent += () =>
            {
                ((BulletWaveChild)bullets[index]).Explode();
                CmdDestroyChild(index);
            };
        }

        float angle = Vector2.Angle(Vector2.right, _direction);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, (_direction.normalized.y < 0 ? -1 : 1) * angle);
    }


    public override void Update()
    {
        base.Update();

        if (bullets == null) return;

        transform.localScale += Vector3.one * speed * Time.deltaTime;

        Debug.Log(transform.localScale);
    }

    [Command(requiresAuthority = false)]
    public void CmdDestroyChild(int index)
    {
        RpcDestroyChild(index);
    }

    [ClientRpc]
    public void RpcDestroyChild(int index)
    {
        if (bullets[index] == null) return;

        ((BulletWaveChild)bullets[index]).Explode();
    }

    protected override void Explode()
    {
        if (bullets == null) return;

        foreach (var b in bullets)
        {
            if (b.gameObject)
            {
                ((BulletWaveChild)b).Explode();
            }
        }
        Destroy(this.gameObject);
    }

}
