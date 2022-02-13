using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Bridge : NetworkBehaviourOwner
{
    public bool serverAuthority = false;
    public GameObject[] parts;
    public GameObject explosion;
    public float explosionAnimationLength = 0.4f;
    public float breakInterval = 0.5f;

    [SyncVar]
    private int _brokenPartsCount = 0;
    private int _localBrokenPartsCount = 0;
    private bool _chainStarted = false;

    public override void ClientStart()
    {
        for(int i = 0; i < _brokenPartsCount; i++)
        {
            parts[i].SetActive(false);
        }
    }

    public override void ServerOnTriggerEnter2D(Collider2D collider)
    {
        if (!collider) return;
        Player player = collider.GetComponent<Player>();
        if (player && !player.isInvader)
        {
            StartChain();
            RpcStartChain();
        }
    }

    public override void ClientOnTriggerEnter2D(Collider2D collider)
    {
        if (!collider) return;
        Player player = collider.GetComponent<Player>();
        if (player && !player.isInvader)
        {
            StartChain();
        }
    }

    [Command(requiresAuthority = false)]
    private void CmdStartChain()
    {
        RpcStartChain();
    }

    [ClientRpc]
    private void RpcStartChain()
    {
        StartChain();
    }

    private void StartChain()
    {
        if (_chainStarted) return;

        _chainStarted = true;
        StartCoroutine(StartChainCoroutine());
    }

    private IEnumerator StartChainCoroutine()
    {
        while(_localBrokenPartsCount < parts.Length)
        {
            StartCoroutine(Break(parts[_localBrokenPartsCount]));
            yield return new WaitForSeconds(breakInterval);
            _localBrokenPartsCount++;
            if (isServer)
            {
                _brokenPartsCount = _localBrokenPartsCount;
            }
        }
    }

    private IEnumerator Break(GameObject part)
    {
        GameObject.Instantiate(explosion).transform.position = part.transform.position;
        part.GetComponentsInChildren<SpriteRenderer>().ToList().ForEach(x => x.enabled = false);
        yield return new WaitForSeconds(explosionAnimationLength);
        part.SetActive(false);
    }

}
