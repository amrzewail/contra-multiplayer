using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelEventsCallbacks : NetworkBehaviour
{

    private void Start()
    {
        LevelEvents.OnPlayerDead += PlayerDeadCallback;
        LevelEvents.OnGameOver += GameOverCallback;
    }

    private void PlayerDeadCallback(uint id)
    {
        var instance = FindObjectsOfType<Player>().First(x => x.identity.netId.Equals(id));
        if (instance.isMine)
        {
            Debug.Log($"{GetType().Name}::PlayerDeadCallback Command to server");
            CmdInvokePlayerDead(id);
        }
    }

    [Command(requiresAuthority = false)]
    private void CmdInvokePlayerDead(uint id)
    {
        Debug.Log($"{GetType().Name}::CmdInvokePlayerDead Server received call. Rpc to clients");
        RpcInvokePlayerDead(id);
    }

    [ClientRpc]
    private void RpcInvokePlayerDead(uint id)
    {
        Player[] players = FindObjectsOfType<Player>();
        var instance = players.First(x => x.identity.netId.Equals(id));
        Debug.Log($"{GetType().Name}::RpcInvokePlayerDead Client received call. Invoke:{!instance.isMine}");
        if (!instance.isMine)
        {
            LevelEvents.OnPlayerDead?.Invoke(id);
        }
    }

    private void GameOverCallback()
    {
        if (isServer)
        {
            RpcOnGameOver();
        }
    }

    [ClientRpc]
    private void RpcOnGameOver()
    {
        if (!isServer)
        {
            LevelEvents.OnGameOver?.Invoke();
        }
    }

}
