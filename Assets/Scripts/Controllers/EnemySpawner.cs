using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : NetworkBehaviourOwner
{

    public GameObject enemy;

    public List<Transform> spawnPoints;

    public float spawnInterval;

    private bool playerOnTrigger => _playerTriggerCount > 0;

    private float _currentSpawnTime;
    private int _playerTriggerCount = 0;


    public override void ServerUpdate()
    {

        _currentSpawnTime += Time.deltaTime;

        if(_currentSpawnTime > spawnInterval)
        {
            Spawn();
            _currentSpawnTime = 0;
        }

    }

    public override void ServerOnTriggerEnter2D(Collider2D collider)
    {
        if (collider.GetComponent<Player>())
        {
            _playerTriggerCount++;
        }
    }


    public override void ServerOnTriggerExit2D(Collider2D collider)
    {
        if (collider.GetComponent<Player>())
        {
            _playerTriggerCount--;
            if (_playerTriggerCount < 0) _playerTriggerCount = 0;
        }
    }

    private void Spawn()
    {
        if (playerOnTrigger)
        {
            GameObject g = Instantiate(enemy);
            g.transform.position = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Count)].position;
            NetworkServer.Spawn(g);
        }
    }
}
