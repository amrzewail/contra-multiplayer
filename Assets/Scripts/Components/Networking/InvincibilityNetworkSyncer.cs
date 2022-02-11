using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvincibilityNetworkSyncer : NetworkBehaviourOwner
{
    public Invincibility target;

    public override void MyStart()
    {
        target.OnStartInvincible.AddListener(StartInvincibleCallback);
    }

    private void StartInvincibleCallback()
    {
        CmdTrigger(identity.netId);
    }


    [Command]
    private void CmdTrigger(uint netId)
    {
        RpcTrigger(netId);
    }

    [ClientRpc]
    private void RpcTrigger(uint netId)
    {
        if (netId == identity.netId && !isMine)
        {
            target.Trigger();
        }
    }
}
