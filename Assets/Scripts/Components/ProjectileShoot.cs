using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileShoot : MonoBehaviour, IShooter
{
    public GameObject projectile;

    public Transform rightPivot;
    public Transform leftPivot;
    public int maxBullets = 2;
    public float cooldownBetweenBullets = 0.25f;
    public float cooldown = 0.5f;

    private int _currentBullets = 0;
    private float _lastShootingTime;

    public bool Shoot(int direction)
    {
        if (Time.time - _lastShootingTime >= cooldown) _currentBullets = 0;
        if (Time.time - _lastShootingTime < cooldownBetweenBullets) return false;
        if(_currentBullets < maxBullets)
        {
            ShootBullet(direction);
            _currentBullets++;
            _lastShootingTime = Time.time;
            return true;
        }
        return false;
    }

    private void ShootBullet(int direction)
    {
        GameObject g = Instantiate(projectile);
        g.GetComponentInChildren<IProjectile>().direction = direction;
        g.transform.position = direction == 1 ? rightPivot.position : leftPivot.position;
    }
}
