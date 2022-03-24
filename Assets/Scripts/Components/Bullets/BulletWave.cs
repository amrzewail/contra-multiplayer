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
    private Vector2[] directions;

    public override void OtherStart()
    {
        var instance = FindObjectsOfType<NetworkBehaviourOwner>().First(x => x.netId.Equals(_shooterId));

        transform.position += instance.transform.position - Vector3.one * 9000;

        bullets = new IBullet[transform.childCount];
        directions = new Vector2[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            bullets[i] = transform.GetChild(i).GetComponent<IBullet>();
        }
    }

    public override void MyStart()
    {
        var instance = FindObjectsOfType<NetworkBehaviourOwner>().First(x => x.netId.Equals(_shooterId));

        transform.position += instance.transform.position - Vector3.one * 9000;
        Invoke("CmdDestroySelf", destroyAfter);

        bullets = new IBullet[transform.childCount];
        directions = new Vector2[transform.childCount];
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

        _direction.Normalize();
        var perp = Vector3.Cross(_direction, new Vector3(0, 0, 1));

        directions[0] = (_direction);

        directions[1] = (_direction + perp * 0.25f);
        directions[2] = (_direction - perp * 0.25f);

        directions[3] = (_direction + perp * 0.5f);
        directions[4] = (_direction - perp * 0.5f);
    }


    public override void MyUpdate()
    {
        if (bullets == null) return;

        //transform.position += Vector3.right * 1f * Time.deltaTime;

        for (int i = 0; i < bullets.Length; i++)
        {
            if (bullets[i] != null)
            {
                bullets[i].gameObject.transform.position += (Vector3)directions[i].normalized * speed * Time.deltaTime;
            }
        }

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
