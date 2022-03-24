using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPowerup : NetworkBehaviourOwner
{
    public int bulletIndex = 1;

    private Rigidbody2D _rb;
    private Collider2D _col;

    public override void ServerStart()
    {
        _col = GetComponent<Collider2D>();
        _rb = GetComponentInChildren<Rigidbody2D>();
        _rb.AddForce(new Vector2(0.1f, 1) * 15, ForceMode2D.Impulse);
        _col.enabled = false;
    }

    public override void ServerFixedUpdate()
    {
        if(_rb.velocity.y > 0)
        {
            _col.enabled = false;
        }
        else
        {
            _col.enabled = true;
        }
    }

    public void TriggerEnter(Collider2D collider)
    {
        Player player = collider.GetComponentInChildren<Player>();
        if (!player) return;
        if (!player.isMine) return;
        player.shooter.AssignBullet(bulletIndex);

        SoundEvents.Play(SFX.Powerup);

        CmdPowerupTaken(identity.netId);
        gameObject.SetActive(false);
    }

    [Command(requiresAuthority = false)]
    private void CmdPowerupTaken(uint id)
    {
        if(id == identity.netId)
        {
            RpcPowerupTaken(id);
        }
    }

    [ClientRpc]
    private void RpcPowerupTaken(uint id)
    {
        if(id == identity.netId)
        {
            Destroy(this.gameObject);
        }
    }
}
