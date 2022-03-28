using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DamageNetworkSyncer : NetworkBehaviourOwner
{

    public Damage target;

    [SyncVar(hook = nameof(OnDamageTypeChange))]
    private DamageType _damageType;

    public override void MyStart()
    {

    }

    public override void OtherStart()
    {
    }

    public override void ServerStart()
    {

        _damageType = target.damageType;

        Debug.Log($"DamageNetworkSyncer::ServerStart id:{netId} damageType:{_damageType} identity:{identity.netId} condition:{identity.netId == netId}");

        RpcSetDamageType(identity.netId, _damageType);
        //_damagePlayer = target.playerDamage;
    }


    public override void ServerUpdate()
    {
        _damageType = target.damageType;
    }

    public override void ClientUpdate()
    {
    }

    public override void ClientStart()
    {

    }

    private void OnDamageTypeChange(DamageType old, DamageType newType) 
    {
        target.damageType = newType;
    }


    [ClientRpc]
    private void RpcSetDamageType(uint netId, DamageType damageType)
    {
        Debug.Log($"DamageNetworkSyncer::RpcSetDamageType id:{netId} damageType:{damageType} identity:{identity.netId} condition:{identity.netId == netId}");
        if (identity.netId == netId)
        {
            target.damageType = damageType;

            bool enableDamage = false;

            if (LevelController.instance.myPlayer.isInvader)
            {
                enableDamage = (target.damageType == DamageType.Player && !isMine);

            }
            else
            {
                enableDamage = isMine || target.damageType == DamageType.Invader;
            }

            Debug.Log($"DamageNetworkSyncer::RpcSetDamageType enableDamage:{enableDamage} damageType:{target.damageType} isInvader:{LevelController.instance.myPlayer.isInvader} isMine:{isMine}");

            target.GetComponentsInChildren<Collider2D>().ToList().ForEach(x => x.enabled = enableDamage);
        }
    }
}
