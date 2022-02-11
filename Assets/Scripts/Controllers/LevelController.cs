using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public static LevelController instance { get; private set; }

    [SerializeField] Transform spawnLocation;
    [SerializeField] Transform bossCameraLocation;

    public bool isBossDefeated = false;

    

    internal void Awake()
    {
        if (instance)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
    }

    public Transform GetSpawnLocation()
    {
        return spawnLocation;
    }

    public Transform GetBossCameraLocation()
    {
        return bossCameraLocation;
    }
}
