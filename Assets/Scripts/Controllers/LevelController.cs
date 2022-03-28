using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.InputSystem.InputAction;

public class LevelController : MonoBehaviour
{
    public static LevelController instance { get; private set; }

    [SerializeField] Transform spawnLocation;
    [SerializeField] Transform[] invaderSpawnLocations;

    [SerializeField] Transform bossCameraLocation;

    public bool isBossDefeated = false;

    private Player _myPlayer;
    private Player _spectateTarget;
    private List<uint> deadPlayersList = new List<uint>();
    
    public Player myPlayer 
    { 
        get
        {
            if (!_myPlayer)
            {
                _myPlayer = FindObjectsOfType<Player>().FirstOrDefault(x => x.isMine);
            }
            return _myPlayer;
        }
    }

    internal void Awake()
    {
        Debug.Log($"LevelController::Awake");
        instance = this;
    }

    internal void Update()
    {
    }

    internal void Start()
    {
        Debug.Log($"LevelController::Start");

        NetworkEvents.OnClientDisconnect += ClientDisconnectCallback;
        LevelEvents.OnPlayerDead += PlayerDeadCallback;
        LevelEvents.OnGameOver += GameOverCallback;
        LevelEvents.StageBossDefeated += StageBossDefeatedCallback;
    }

    internal void OnDestroy()
    {
        NetworkEvents.OnClientDisconnect -= ClientDisconnectCallback;
        LevelEvents.OnPlayerDead -= PlayerDeadCallback;
        LevelEvents.OnGameOver -= GameOverCallback;
        LevelEvents.StageBossDefeated -= StageBossDefeatedCallback;
    }

    internal void LateUpdate()
    {
        if (myPlayer == null) return;

        if(myPlayer.state == Player.State.Spectate && _spectateTarget)
        {
            myPlayer.transform.position = _spectateTarget.transform.position;
        }
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

    private void StageBossDefeatedCallback()
    {
        Debug.Log($"LevelController::StageBossDefeatedCallback Is Server:{NetworkClient.isHostClient} Game Over");

        var myPlayer = FindObjectsOfType<Player>().First(x => x.isMine);

        if (myPlayer.isInvader)
        {
            LevelEvents.OnDefeat?.Invoke();
        }
        else
        {
            PlayerPrefs.SetInt("DidKillBoss", 1);
            LevelEvents.OnVictory?.Invoke();
        }

        StartCoroutine(GameOverCoroutine());
    }

    private void PlayerDeadCallback(uint id)
    {
        deadPlayersList.Add(id);
        Debug.Log($"LevelController::PlayerDeadCallback Is Server:{NetworkClient.isHostClient} Player:{id} died.");

        if (NetworkClient.isHostClient)
        {
            Player[] players = FindObjectsOfType<Player>();
            bool allDead = true;
            for (int i = 0; i < players.Length; i++)
            {
                if(players[i].isInvader == false && !deadPlayersList.Contains(players[i].identity.netId))
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
        else
        {
            Debug.Log($"LevelController::PlayerDeadCallback Is Invader:{myPlayer.isInvader} Player:{id}.");

            if (myPlayer.isInvader && myPlayer.identity.netId == id)
            {
                Debug.Log($"LevelController::PlayerDeadCallback Invoke Defeat.");
                LevelEvents.OnDefeat?.Invoke();
                StartCoroutine(GameOverCoroutine());
            }
        }
    }

    private void GameOverCallback()
    {
        Debug.Log($"LevelController::GameOverCallback Is Server:{NetworkClient.isHostClient} Game Over");

        if (myPlayer.isInvader)
        {
            LevelEvents.OnVictory?.Invoke();
        }
        else
        {
            LevelEvents.OnDefeat?.Invoke();
        }

        StartCoroutine(GameOverCoroutine());
    }

    public void StartSpectate()
    {
        var spectates = FindObjectsOfType<Player>().Where(x => x.isInvader == false && !x.isMine && !deadPlayersList.Contains(x.identity.netId));
        if(spectates != null && spectates.Count() > 0)
        {
            _spectateTarget = spectates.ElementAt(0);
        }
    }

    public void ChangeSpectateTarget(int direction)
    {
        direction = direction > 0 ? 1 : (direction < 0 ? -1 : 0);
        var spectates = FindObjectsOfType<Player>().Where(x => x.isInvader == false && !x.isMine && !deadPlayersList.Contains(x.identity.netId));
        if (spectates != null && spectates.Count() > 0)
        {
            if (!_spectateTarget) _spectateTarget = spectates.ElementAt(0);
            else
            {
                int currentIndex = spectates.ToList().IndexOf(_spectateTarget);
                currentIndex += direction;
                if (currentIndex < 0) currentIndex = spectates.Count() - 1;
                currentIndex = currentIndex % spectates.Count();
                _spectateTarget = spectates.ElementAt(currentIndex);
            }
        }
    }


    private IEnumerator GameOverCoroutine()
    {
        yield return new WaitForSeconds(5);
        ExitLevel();
    }
}
