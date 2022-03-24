using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelEvents : MonoBehaviour
{
    public static Action StageBossStarted;
    public static Action StageBossDefeated;
    public static Action<uint> OnPlayerDead;
    public static Action OnGameOver;

    public static Action OnVictory;
    public static Action OnDefeat;

    private void Awake()
    {
        ResetEvents();
    }

    private void OnDestroy()
    {
        ResetEvents();
    }

    private void ResetEvents()
    {
        StageBossStarted = null;
        StageBossDefeated = null;
        OnPlayerDead = null;
        OnGameOver = null;
        OnVictory = null;
        OnDefeat = null;
    }
}
