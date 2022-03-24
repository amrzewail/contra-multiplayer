using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverOverlays : MonoBehaviour
{
    [SerializeField] GameObject victoryOverlay;
    [SerializeField] GameObject defeatOverlay;

    void Start()
    {
        LevelEvents.OnVictory += VictoryCallback;
        LevelEvents.OnDefeat += DefeatCallback;
    }

    void OnDestroy()
    {

        LevelEvents.OnVictory -= VictoryCallback;
        LevelEvents.OnDefeat -= DefeatCallback;
    }

    private void VictoryCallback()
    {
        victoryOverlay.SetActive(true);
        SoundEvents.StopBackground();
        SoundEvents.Play(SFX.Victory);
    }

    private void DefeatCallback()
    {
        defeatOverlay.SetActive(true);
        SoundEvents.StopBackground();
        SoundEvents.Play(SFX.Defeat);
    }
}
