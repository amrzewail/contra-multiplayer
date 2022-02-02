using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : NetworkBehaviourOwner
{
    [SerializeField] GameObject playerObject;

    public struct CharacterMessage : NetworkMessage
    {

    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        NetworkServer.RegisterHandler<CharacterMessage>(SpawnPlayerCallback);
    }

    public override void MyStart()
    {
        NetworkClient.Send(new CharacterMessage() { });


        SceneManager.sceneLoaded += SceneLoadedCallback;
    }

    public override void MyOnDestroy()
    {
        SceneManager.sceneLoaded -= SceneLoadedCallback;
    }


    private void SceneLoadedCallback(Scene scene, LoadSceneMode mode)
    {
        if(scene.buildIndex == 1)
        {
            NetworkServer.Spawn(playerObject);
        }
    }

    private void SpawnPlayerCallback(NetworkConnection conn, CharacterMessage message)
    {
        GameObject instantiatedObject = Instantiate(playerObject);

        NetworkServer.AddPlayerForConnection(conn, instantiatedObject);
    }

    public void SpawnPlayer()
    {
        if (isServer)
        {
            RpcSpawnPlayer(identity.netId);
        }
        else
        {
            CmdSpawnPlayer(identity.netId);
        }
    }

    [ClientRpc]
    public void RpcSpawnPlayer(uint id)
    {
        if(identity.netId == id)
        {
            
        }
    }

    [Command]
    public void CmdSpawnPlayer(uint id)
    {
        RpcSpawnPlayer(id);
    }
}
