using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEvents : MonoBehaviour
{
    public static Action StageBossStarted;
    public static Action StageBossDefeated;

    private void Awake()
    {
        StageBossDefeated = null;
    }

    private void OnDestroy()
    {
        StageBossDefeated = null;
    }
}
