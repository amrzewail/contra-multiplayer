using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireratePowerup : NetworkBehaviourOwner
{
    public int increase = 4;

    public void TriggerEnter(Collider2D collider)
    {
        Player player = collider.GetComponentInChildren<Player>();
        if (!player) return;
        if (!player.isMine) return;
        player.shooter.IncreaseFireRate(increase);

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
