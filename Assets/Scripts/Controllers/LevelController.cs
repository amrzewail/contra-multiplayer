using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public static LevelController instance { get; private set; }

    [SerializeField] Transform spawnLocation;

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
}
