using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectEnabler : NetworkBehaviourOwner
{

    public GameObject target;

    public float checkInterval;

    public bool destroyAfterEnable = false;

    private bool playerOnTrigger => _playerTriggerCount > 0;

    private float _currentSpawnTime;
    private int _playerTriggerCount = 0;

    public override void ClientStart()
    {
        target.gameObject.SetActive(false);
        CmdHideOnStart(identity.netId);
    }

    public override void ServerStart()
    {
        RpcHideOnStart(identity.netId);
    }

    [Command(requiresAuthority = false)]
    private void CmdHideOnStart(uint id)
    {
        RpcHideOnStart(id);
    }

    [ClientRpc]
    private void RpcHideOnStart(uint id)
    {
        if (!this || !this.gameObject || !target) return;
        if(id == identity.netId)
        {
            target.gameObject.SetActive(false);
        }
    }

    public override void ServerUpdate()
    {

        _currentSpawnTime += Time.deltaTime;

        if(_currentSpawnTime > checkInterval)
        {
            Spawn();
            _currentSpawnTime = 0;
        }

    }

    public override void ServerOnTriggerEnter2D(Collider2D collider)
    {
        Player player;
        if ((player = collider.GetComponent<Player>()))
        {
            if (player.isInvader) return;

            _playerTriggerCount++;
        }
    }


    public override void ServerOnTriggerExit2D(Collider2D collider)
    {
        Player player;
        if ((player = collider.GetComponent<Player>()))
        {
            if (player.isInvader) return;

            _playerTriggerCount--;
            if (_playerTriggerCount < 0) _playerTriggerCount = 0;
        }
    }

    [Server]
    private void Spawn()
    {
        if (playerOnTrigger)
        {
            RpcEnableObject(identity.netId);
        }
    }

    [ClientRpc]
    private void RpcEnableObject(uint id)
    {
        if(id == identity.netId)
        {
            target.gameObject.SetActive(true);
            Destroy(this.gameObject);
        }
    }

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        if (!target) return;
        Gizmos.DrawLine(transform.position, target.transform.position);
    }

#endif
}
