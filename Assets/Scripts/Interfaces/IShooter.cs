using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IShooter
{
    public bool Shoot(AimDirection direction);
    public bool Shoot(AimDirection direction, Vector2 axis);

    public void AssignBullet(int index);

    public IBullet GetBullet();

    public UnityEvent<int> BulletChanged { get; set; }

    public Transform GetShootingPoint(AimDirection direction);

    public void IncreaseFireRate(int addedRate);

    public void ResetFireRate();

    UnityEvent<AimDirection, Vector2> OnShoot { get; set; }

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
    JumpStraight,
    WaterStraight,
    WaterHigh,
    WaterUp,
    Extra1
}