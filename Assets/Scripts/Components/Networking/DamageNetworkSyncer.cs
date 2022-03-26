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

        if(_damageType == DamageType.Player)
        {
            target.GetComponentsInChildren<Collider2D>().ToList().ForEach(x => x.enabled = true);
            Debug.Log($"DamageNetworkSyncer::MyStart time:{Time.time} This is my bullet therefore Enable");

        }
    }

    public override void OtherStart()
    {
        if (_damageType == DamageType.Player)
        {
            target.GetComponentsInChildren<Collider2D>().ToList().ForEach(x => x.enabled = false);
            Debug.Log($"DamageNetworkSyncer::MyStart time:{Time.time} Not my bullet therefore Disable");
        }

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
    }


    [ClientRpc]
    private void RpcSetDamageType(uint netId)
    {
        Debug.Log($"DamageNetworkSyncer::RpcSetDamageType time:{Time.time} id:{netId} identity:{identity.netId} condition:{identity.netId == netId}");
        if (identity.netId == netId)
        {
            target.damageType = _damageType;
            //target.GetComponentsInChildren<Collider2D>().ToList().ForEach(x => x.enabled = true);
        }
    }
}
