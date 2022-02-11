using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateObject : NetworkBehaviourOwner
{
    private bool _didInstantiate = false;

    public bool activateOnInstantiate = false;

    public GameObject target;

    public void Instantiate()
    {
        _didInstantiate = false;
        InstantiateInternal(target);
        CmdInstantiate(0);
    }

    [Command(requiresAuthority = false)]
    private void CmdInstantiate(int conn)
    {
        RpcInstantiate(conn);
    }

    [ClientRpc]
    private void RpcInstantiate(int conn)
    {
        InstantiateInternal(target);
    }

    private void InstantiateInternal(GameObject gameObj)
    {
        if (!_didInstantiate)
        {
            gameObj = GameObject.Instantiate(gameObj);
            gameObj.transform.position = this.transform.position;
            gameObj.transform.rotation = this.transform.rotation;
            if (activateOnInstantiate) gameObj.SetActive(true);
            _didInstantiate = true;
        }
    }

}
