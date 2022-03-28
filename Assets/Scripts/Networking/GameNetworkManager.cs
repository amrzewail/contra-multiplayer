using kcp2k;
using Mirror;
using Mirror.Discovery;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameNetworkManager : NetworkManager
{
    public static new GameNetworkManager singleton => (GameNetworkManager)NetworkManager.singleton;

    public NetworkDiscovery networkDiscovery;

    private int _numberOfPlayers = 0;
    public int numberOfPlayers => _numberOfPlayers;

    public struct CreateCharacterMessage : NetworkMessage
    {
        public Color color;
        public bool isInvader;
    }

    private bool _readyToJoin=false;
    private bool _cancelJoining = false;

    private Dictionary<int, CreateCharacterMessage> _connectedPlayers = new Dictionary<int, CreateCharacterMessage>();

    public async Task<bool> StartSinglePlayer()
    {

        if (transport is KcpTransport)
        {
            ((KcpTransport)transport).Port = 0;
        }
        else if (transport is LatencySimulation)
        {
            ((KcpTransport)((LatencySimulation)transport).wrap).Port = 0;
        }
        StartHost();
        return true;
    }

    public async Task<bool> HostMultiplayer()
    {

        if(transport is KcpTransport)
        {
            ((KcpTransport)transport).Port = 7777;
        }else if (transport is LatencySimulation)
        {
            ((KcpTransport)((LatencySimulation)transport).wrap).Port = 7777;
        }
        return await InternalHostMultiplayer(0);

    }
    public async Task<bool> JoinMultiplayer()
    {
        _readyToJoin = false;
        _cancelJoining = false;

        if (transport is KcpTransport)
        {
            ((KcpTransport)transport).Port = 7777;
        }
        else if (transport is LatencySimulation)
        {
            ((KcpTransport)((LatencySimulation)transport).wrap).Port = 7777;
        }
        networkDiscovery.StartDiscovery();
        networkDiscovery.OnServerFound.AddListener(OnDiscoveredServer);

        await Task.Run(async () =>
        {
            while (!_readyToJoin && !_cancelJoining)
            {
                await Task.Delay(100);
            }
        });

        return !_cancelJoining;
    }

    public void StopClientOrServer()
    {
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopHost();
            networkDiscovery.StopDiscovery();
            _numberOfPlayers = 0;
        }
        // stop client if client-only
        else if (NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopClient();
            networkDiscovery.StopDiscovery();
            _numberOfPlayers = 0;
        }
    }

    private async Task<bool> InternalHostMultiplayer(int count)
    {
        try
        {
            StartHost();
            networkDiscovery.AdvertiseServer();
            return true;
        }
        catch (SocketException e)
        {
            if (count == 100)
            {
                Debug.Log("No available ports");
                return false;
            }
            else
            {
                await System.Threading.Tasks.Task.Delay(100);
                if (transport is KcpTransport)
                {
                    ((KcpTransport)transport).Port++;
                }
                else if (transport is LatencySimulation)
                {
                    ((KcpTransport)((LatencySimulation)transport).wrap).Port++;
                }
                return await InternalHostMultiplayer(count+1);
            }
        }
    }


    public void StopSearching()
    {
        _cancelJoining = true;
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

        NetworkEvents.OnHostConnect?.Invoke();
    }

    public override void OnClientConnect()
    {
        base.OnClientConnect();


        SceneManager.sceneLoaded += SceneLoadedCallback;

        _readyToJoin = true;

        NetworkEvents.OnClientConnect?.Invoke();

    }
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
        if (_connectedPlayers.ContainsKey(conn.connectionId))
        {
            if(_connectedPlayers[conn.connectionId].isInvader == false)
            {
                _numberOfPlayers--;
                if (_numberOfPlayers < 0) _numberOfPlayers = 0;
            }
            _connectedPlayers.Remove(conn.connectionId);
            Debug.Log($"{GetType().Name}::{nameof(OnServerDisconnect)} PlayerConnection:{conn.connectionId} NumOfPlayers:{_numberOfPlayers}");
        }
    }

    public override void OnClientDisconnect()
    {
        NetworkEvents.OnClientDisconnect?.Invoke();
    }


    private async void SceneLoadedCallback(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= SceneLoadedCallback;

        Color selectedColor = Color.blue;
        if (ColorUtility.TryParseHtmlString(PlayerPrefs.GetString("player_color", $"#{ColorUtility.ToHtmlStringRGB(Color.blue)}"), out selectedColor))
        {
            
        }
        // you can send the message here, or wherever else you want
        CreateCharacterMessage characterMessage = new CreateCharacterMessage
        {
            color = selectedColor,
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
            Debug.Log($"{this.GetType().Name}::{nameof(CreateCharacterCallback)} controller instance:{LevelController.instance}");
            playerObj.transform.position = LevelController.instance.GetSpawnLocation().position;
            _numberOfPlayers++;
        }

        // call this to use this gameobject as the primary controller
        NetworkServer.AddPlayerForConnection(conn, playerObj);
        NetworkServer.SetClientReady(conn);
        NetworkServer.SpawnObjects();

        if (_connectedPlayers == null) _connectedPlayers = new Dictionary<int, CreateCharacterMessage>();
        _connectedPlayers[conn.connectionId] = message;
    }
}
