using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.InputSystem.InputAction;

public class LevelController : NetworkBehaviour
{
    public static LevelController instance { get; private set; }

    [SerializeField] Transform spawnLocation;
    [SerializeField] Transform[] invaderSpawnLocations;

    [SerializeField] Transform bossCameraLocation;

    public bool isBossDefeated = false;

    [SyncVar] private int _playerDeadCount = 0;
    public int playerDeadCount { get => _playerDeadCount; }

    private List<uint> deadPlayersList = new List<uint>();
    

    internal void Awake()
    {
        instance = this;
    }

    internal void Start()
    {
        NetworkEvents.OnClientDisconnect += ClientDisconnectCallback;
        LevelEvents.OnPlayerDead += PlayerDeadCallback;
        LevelEvents.OnGameOver += GameOverCallback;
    }

    internal void OnDestroy()
    {
        NetworkEvents.OnClientDisconnect -= ClientDisconnectCallback;
        LevelEvents.OnPlayerDead -= PlayerDeadCallback;
        LevelEvents.OnGameOver -= GameOverCallback;
    }

    public Transform GetSpawnLocation()
    {
        return spawnLocation;
    }

    public Transform GetInvaderSpawnLocation()
    {
        Player[] players = Object.FindObjectsOfType<Player>().ToList().Where(x => x.isInvader == false).ToArray();
        Vector2 mean = Vector2.zero;
        for(int i = 0; i < players.Length; i++)
        {
            mean += (Vector2)players[i].transform.position;
        }
        mean /= players.Length;

        int closestIndex = 0;
        float closestDistance = Vector2.Distance(invaderSpawnLocations[0].position, mean);

        for(int i = 1; i < invaderSpawnLocations.Length; i++)
        {
            float dist = Vector2.Distance(invaderSpawnLocations[i].position, mean);
            if(dist < closestDistance)
            {
                closestDistance = dist;
                closestIndex = i;
            }
        }

        return invaderSpawnLocations[closestIndex];

    }

    public Transform GetBossCameraLocation()
    {
        return bossCameraLocation;
    }

    public void StartKeyCallback(CallbackContext ctx)
    {
        if (ctx.performed)
        {
            ExitLevel();
        }
    }

    public void ExitLevel()
    {
        GameNetworkManager.singleton.StopClientOrServer();
        SceneManager.LoadScene((int)SceneIndex.MainMenu);
    }

    private void ClientDisconnectCallback()
    {
        GameNetworkManager.singleton.StopClientOrServer();
        SceneManager.LoadScene((int)SceneIndex.MainMenu);
    }

    private void PlayerDeadCallback(uint id)
    {
        NetworkIdentity identity = GetComponentInChildren<NetworkIdentity>();
        deadPlayersList.Add(id);
        if (identity.isServer)
        {
            _playerDeadCount++;
            Debug.Log($"LevelController::PlayerDeadCallback Is Server:{identity.isServer} Player:{id} died. Death count:{playerDeadCount}");
            Player[] players = FindObjectsOfType<Player>();
            bool allDead = true;
            for (int i = 0; i < players.Length; i++)
            {
                if(!deadPlayersList.Contains(players[i].identity.netId))
                {
                    allDead = false;
                    break;
                }
            }
            if (allDead)
            {
                LevelEvents.OnGameOver?.Invoke();
            }
        }
    }

    private void GameOverCallback()
    {
        Debug.Log($"LevelController::GameOverCallback Is Server:{NetworkClient.isHostClient} Game Over");

    }
}
