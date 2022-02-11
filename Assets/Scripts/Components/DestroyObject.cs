using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObject : NetworkBehaviourOwner
{
    private bool _didDestroy = false;

    public GameObject target;

    public void Destroy()
    {
        _didDestroy = false;
        DestroyInternal(target);
        CmdDestroy(0, target);
    }

    [Command(requiresAuthority = false)]
    private void CmdDestroy(int conn, GameObject g)
    {
        RpcDestroy(conn, g);
    }

    [ClientRpc]
    private void RpcDestroy(int conn, GameObject g)
    {
        DestroyInternal(g);
    }

    private void DestroyInternal(GameObject gameObj)
    {
        if (!_didDestroy)
        {
            GameObject.Destroy(gameObj);
            _didDestroy = true;
        }
    }

}
