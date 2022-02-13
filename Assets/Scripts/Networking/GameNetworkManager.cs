using kcp2k;
using Mirror;
using Mirror.Discovery;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameNetworkManager : NetworkManager
{
    public static new GameNetworkManager singleton => (GameNetworkManager)NetworkManager.singleton;

    public NetworkDiscovery networkDiscovery;


    public struct CreateCharacterMessage : NetworkMessage
    {
        public Color color;
        public bool isInvader;
    }




    public void StartSinglePlayer()
    {
        ((KcpTransport)transport).Port = 0;
        StartHost();
    }

    public void HostMultiplayer()
    {
        ((KcpTransport)transport).Port = 7777;
        InternalHostMultiplayer(0);
    }

    private async void InternalHostMultiplayer(int count)
    {
        try
        {
            StartHost();
            networkDiscovery.AdvertiseServer();
        }
        catch (SocketException e)
        {
            if (count == 100)
            {
                Debug.Log("No available ports");
            }
            else
            {
                await System.Threading.Tasks.Task.Delay(100);
                ((KcpTransport)transport).Port++;
                InternalHostMultiplayer(count+1);
            }
        }
    }

    public void JoinMultiplayer()
    {
        ((KcpTransport)transport).Port = 7777;
        networkDiscovery.StartDiscovery();
        networkDiscovery.OnServerFound.AddListener(OnDiscoveredServer);
    }

    public void StopSearching()
    {
        networkDiscovery.StopDiscovery();
    }

    private void OnDiscoveredServer(ServerResponse info)
    {
        networkDiscovery.StopDiscovery();
        StartClient(info.uri);
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        NetworkServer.RegisterHandler<CreateCharacterMessage>(CreateCharacterCallback);
        NetworkServer.SetAllClientsNotReady();
    }

    public override void OnClientConnect()
    {
        base.OnClientConnect();


        SceneManager.sceneLoaded += SceneLoadedCallback;

        SceneManager.LoadScene((int)SceneIndex.Game);

    }

    private void SceneLoadedCallback(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= SceneLoadedCallback;

        // you can send the message here, or wherever else you want
        CreateCharacterMessage characterMessage = new CreateCharacterMessage
        {
            color = new UnityEngine.Color(Random.Range(0f, 1f), Random.Range(0f, 1f),Random.Range(0f, 1f)),
            isInvader = GameManager.instance.isInvader
        };

        NetworkClient.Send(characterMessage);
    }

    void CreateCharacterCallback(NetworkConnection conn, CreateCharacterMessage message)
    {
        // playerPrefab is the one assigned in the inspector in Network
        // Manager but you can use different prefabs per race for example
        GameObject playerObj = Instantiate(playerPrefab);

        playerObj.GetComponent<PlayerIdentity>().SetColor(message.color);
        playerObj.GetComponent<PlayerIdentity>().SetName($"Player {NetworkServer.connections.Count}");

        if (message.isInvader)
        {
            playerObj.GetComponent<PlayerIdentity>().SetAsInvader();
            playerObj.transform.position = LevelController.instance.GetInvaderSpawnLocation().position;
        }
        else
        {
            playerObj.transform.position = LevelController.instance.GetSpawnLocation().position;
        }
        // call this to use this gameobject as the primary controller
        NetworkServer.AddPlayerForConnection(conn, playerObj);
        NetworkServer.SetClientReady(conn);
        NetworkServer.SpawnObjects();
    }
}
