using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShooterNetworkSyncer : NetworkBehaviourOwner
{
    [SerializeField] [RequireInterface(typeof(IShooter))] Object _shooter;

    public IShooter shooter => (IShooter)_shooter;

    public override void MyStart()
    {
        shooter.OnShoot.AddListener(ShootCallback);
        shooter.BulletChanged.AddListener(BulletChangedCallback);
    }

    private void ShootCallback(AimDirection direction)
    {
        var point = shooter.GetShootingPoint(direction);
        if (isServer)
        {
            RpcShoot(identity.netId, point.position - transform.position, point.forward);
        }
        else
        {
            CmdShoot(identity.netId, point.position - transform.position, point.forward);
        }
    }

    [ClientRpc]
    public void RpcShoot(uint netID, Vector3 position, Vector3 direction)
    {
        Shoot(netID, position, direction);
    }

    [Command(channel = Channels.Unreliable)]
    public void CmdShoot(uint netID, Vector3 position, Vector3 direction)
    {
        Shoot(netID, position, direction);
    }

    private void Shoot(uint netID, Vector3 position, Vector3 direction)
    {
        if (isServer)
        {
            var instance = FindObjectsOfType<ShooterNetworkSyncer>().First(x => x.identity.netId.Equals(netID));

            GameObject g = Instantiate(shooter.GetBullet().gameObject, position, Quaternion.identity);
            g.GetComponent<IBullet>().SetDirection(direction);
            g.GetComponent<IBullet>().SetShooterId(netID);
            var connection = instance.connectionToClient;
            NetworkServer.Spawn(g, connection);
        }
    }



    private void BulletChangedCallback(int index)
    {
        if (isServer)
        {
            RpcChangeBullet(netId, index);
        }
        else
        {
            CmdChangeBullet(netId, index);
        }
    }

    [Command(channel = Channels.Unreliable)]
    public void CmdChangeBullet(uint netId, int index)
    {
        RpcChangeBullet(netId, index);
    }

    [ClientRpc]
    public void RpcChangeBullet(uint netId, int index)
    {
        if (identity.netId.Equals(netId) && !isMine)
        {
            shooter.AssignBullet(index);
        }
    }
}
