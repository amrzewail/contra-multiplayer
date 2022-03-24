using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class ProjectileShooter : MonoBehaviourOwner, IShooter
{

    [SerializeField] [RequireInterface(typeof(IBullet))] Object _currentBullet;
    [SerializeField] [RequireInterface(typeof(IBullet))] Object[] _bullets;


    [SerializeField] UnityEvent<AimDirection, Vector2> _OnShoot;

    [SerializeField] UnityEvent<int> _BulletChanged;


    private List<ShootPoint> _points;

    public IBullet currentBullet => (IBullet)_currentBullet;

    public UnityEvent<AimDirection, Vector2> OnShoot { get => _OnShoot; set => _OnShoot = value; }

    public UnityEvent<int> BulletChanged { get => _BulletChanged; set => _BulletChanged = value; }

    private int _currentBullets = 0;
    private float _currentReloadTime;
    private float _lastFireTime = 0;
    private int _extraFireRate = 0;

    public override void MyStart()
    {
        base.MyAwake();

        _points = GetComponentsInChildren<ShootPoint>().ToList();

        Reload();
    }

    public override void MyUpdate()
    {
        if(_currentBullets < currentBullet.bullets)
        {
            _currentReloadTime += Time.deltaTime;
        }
        else
        {
            _currentReloadTime = 0;
        }

        float interval = currentBullet.reloadInterval - _extraFireRate * 0.25f / currentBullet.reloadInterval;
        if (_currentReloadTime > interval)
        {
            Reload();
        }

    }

    public bool Shoot(AimDirection direction)
    {
        //GameObject g = Instantiate(bullet.gameObject, point.transform.position, point.transform.rotation);
        if (_currentBullets > 0)
        {
            if (Time.time - _lastFireTime > 1f / (currentBullet.fireRate + _extraFireRate))
            {
                _lastFireTime = Time.time;
                _currentBullets--;

                var point = GetShootingPoint(direction);

                OnShoot?.Invoke(direction, point.forward);

                return true;
            }
        }
        return false;
    }
    public bool Shoot(AimDirection direction, Vector2 axis)
    {
        return Shoot(direction);
    }

    public void AssignBullet(int index)
    {
        _currentBullet = _bullets.Single(x => ((IBullet)x).index == index);
        BulletChanged?.Invoke(index);
    }

    public void IncreaseFireRate(int add)
    {
        _extraFireRate = add;
    }

    public void ResetFireRate()
    {
        _extraFireRate = 0;
    }

    public IBullet GetBullet()
    {
        return currentBullet;
    }

    public Transform GetShootingPoint(AimDirection direction)
    {
        var point = _points.Single(x => x.direction == direction);
        //if(point.transform.position.x > transform.position.x)
        //{
        //    point.transform.eulerAngles = new Vector3(point.transform.eulerAngles.x, 0, point.transform.eulerAngles.z);
        //}
        //else
        //{
        //    point.transform.eulerAngles = new Vector3(point.transform.eulerAngles.x, 180, point.transform.eulerAngles.z);
        //}
        return point.transform;
    }

    private void Reload()
    {
        _currentBullets = currentBullet.bullets;
    }
}
