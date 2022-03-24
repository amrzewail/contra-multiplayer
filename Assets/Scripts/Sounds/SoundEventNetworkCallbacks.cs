using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEventNetworkCallbacks : NetworkBehaviour
{
    private string Guid;

    [SerializeField] SoundManager soundManager;

    void Start()
    {
        Guid = System.Guid.NewGuid().ToString();

        SoundEvents.Play += PlayCallback;   
    }

    private void PlayCallback(SFX sfx)
    {
        if (isServer)
        {
            RpcPlaySound(Guid, sfx);
        }
        else
        {
            CmdPlaySound(Guid, sfx);
        }
    }

    [ClientRpc]
    private void RpcPlaySound(string senderGuid, SFX sfx)
    {
        if (senderGuid != Guid)
        {
            soundManager.PlayCallback(sfx);
        }
    }

    [Command(requiresAuthority = false)]
    private void CmdPlaySound(string senderGuid, SFX sfx)
    {
        RpcPlaySound(senderGuid, sfx);
    }
}
