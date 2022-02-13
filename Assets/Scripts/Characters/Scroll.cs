using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scroll : NetworkBehaviourOwner
{

    [SerializeField] float _horizontalSpeed = 5;
    [SerializeField] float _verticalRange = 2;
    [SerializeField] float _verticalFrequency = 4;
    [SerializeField] GameObject _powerup;
    [SerializeField] GameObject _explosion;

    private IHitbox _hitbox;

    private Vector3 _startPosition;
    private float _appearTime;
    private bool _hasSpawned = false;

    public override void ServerStart()
    {
        _hitbox = GetComponentInChildren<IHitbox>();

        _appearTime = Time.time;
        _startPosition = transform.position;
        _hasSpawned = false;
    }

    public override void ClientStart()
    {
        _hitbox = GetComponentInChildren<IHitbox>();
    }

    public override void ServerUpdate()
    {
        Vector3 pos = transform.position;
        pos.x += _horizontalSpeed * Time.deltaTime;

        pos.y = _startPosition.y + Mathf.Sin((Time.time - _appearTime) * _verticalFrequency) * _verticalRange;

        transform.position = pos;

        if(_hitbox.IsHit(out int _))
        {
            GameObject.Instantiate(_explosion, transform.position, Quaternion.identity);
            Destroy(this.gameObject);
            CmdHit(identity.netId);
        }
    }

    public override void ClientUpdate()
    {
        if (_hitbox.IsHit(out int _))
        {
            GameObject.Instantiate(_explosion, transform.position, Quaternion.identity);
            Destroy(this.gameObject);
            CmdHit(identity.netId);
        }
    }

    [Server]
    public void Open()
    {
        GameObject powerup = GameObject.Instantiate(_powerup, this.transform.position, Quaternion.identity);
        NetworkServer.Spawn(powerup);
    }

    [ClientRpc]
    private void RpcHit(uint netID)
    {
        if (this && this.gameObject)
        {
            if (netId == identity.netId)
            {
                GameObject.Instantiate(_explosion, transform.position, Quaternion.identity);
                Destroy(this.gameObject);
            }
        }
    }

    [Command(requiresAuthority = false)]
    private void CmdHit(uint netID)
    {
        if(netID == identity.netId)
        {
            if (!_hasSpawned)
            {
                _hasSpawned = true;
                Open();
                RpcHit(netID);
            }
        }
    }

}
