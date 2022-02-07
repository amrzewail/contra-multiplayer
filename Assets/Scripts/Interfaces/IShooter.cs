using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IShooter
{
    public bool Shoot(AimDirection direction);

    public void AssignBullet(int index);

    public IBullet GetBullet();

    public UnityEvent<int> BulletChanged { get; set; }

    public Transform GetShootingPoint(AimDirection direction);

    UnityEvent<AimDirection> OnShoot { get; set; }
}
public enum AimDirection
{
    Up,
    High,
    Straight,
    Low,
    Down,
    StraightUp,
    StraightDown,
    JumpStraight
}