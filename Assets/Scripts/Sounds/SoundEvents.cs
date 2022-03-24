using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SoundEvents
{

    public static Action<SFX> Play;

    public static Action StopBackground;


}

public enum SFX
{
    Victory,
    Defeat,
    Death,
    BossDefeat,
    EnemyGun,
    EnemyGun2,
    EnemyHit,
    Explode,
    Explode2,
    MachineGun,
    Rapid,
    Rapid2,
    Rifle,
    Spread,
    EnemyHit2
}
