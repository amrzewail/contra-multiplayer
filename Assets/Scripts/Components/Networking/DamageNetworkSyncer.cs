using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DamageNetworkSyncer : NetworkBehaviourOwner
{

    public Damage target;

    [SyncVar]
    private DamageType _damageType;

    public override void MyStart()
    {
        target.GetComponentsInChildren<Collider2D>().ToList().ForEach(x => x.enabled = false);

    }

    public override void ServerStart()
    {
        _damageType = target.damageType;
        RpcSetDamageType(identity.netId);
        //_damagePlayer = target.playerDamage;
    }


    public override void ServerUpdate()
    {
        _damageType = target.damageType;
    }

    public override void ClientUpdate()
    {
        target.damageType = _damageType;
    }

    public override void ClientStart()
    {
        target.damageType = _damageType;
        //if (applyInvaderDamageInstantly && isMine && GameManager.instance.isInvader)
        //{
        //    target.damageType = DamageType.Invader;
        //}
    }


    [ClientRpc]
    private void RpcSetDamageType(uint netId)
    {
        if (identity.netId == netId)
        {
            target.damageType = _damageType;
            target.GetComponentsInChildren<Collider2D>().ToList().ForEach(x => x.enabled = true);
        }
    }
}
