using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBullet
{
    int index { get; }

    int bullets { get; }

    float reloadInterval { get; }
    int fireRate { get; }
    bool isContinuous { get; }

    GameObject gameObject { get; }

    void SetDirection(Vector2 direction);

    void SetShooterId(uint id);
}
