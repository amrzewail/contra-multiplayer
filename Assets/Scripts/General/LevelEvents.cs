using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelEvents : NetworkBehaviour
{
    public static Action StageBossStarted;
    public static Action StageBossDefeated;
    public static Action<uint> OnPlayerDead;
    public static Action OnGameOver;

    private void Awake()
    {
        ResetEvents();

        OnPlayerDead += PlayerDeadCallback;
        OnGameOver += GameOverCallback;
    }

    private void OnDestroy()
    {
        ResetEvents();
    }

    private void ResetEvents()
    {
        StageBossStarted = null;
        StageBossDefeated = null;
        OnPlayerDead = null;
        OnGameOver = null;
    }

    private void PlayerDeadCallback(uint id)
    {
        var instance = FindObjectsOfType<Player>().First(x => x.identity.netId.Equals(id));
        if (instance.isMine)
        {
            Debug.Log("LevelEvents::PlayerDeadCallback Command to server");
            CmdInvokePlayerDead(id);
        }
    }

    [Command(requiresAuthority = false)]
    private void CmdInvokePlayerDead(uint id)
    {
        Debug.Log("LevelEvents::CmdInvokePlayerDead Server received call. Rpc to clients");
        RpcInvokePlayerDead(id);
    }

    [ClientRpc]
    private void RpcInvokePlayerDead(uint id)
    {
        Player[] players = FindObjectsOfType<Player>();
        var instance = players.First(x => x.identity.netId.Equals(id));
        Debug.Log($"LevelEvents::RpcInvokePlayerDead Client received call. Invoke:{!instance.isMine}");
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
            OnGameOver?.Invoke();
        }
    }

}
