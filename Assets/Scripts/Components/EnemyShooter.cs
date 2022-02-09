using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class EnemyShooter : MonoBehaviourOwner, IShooter
{

    [SerializeField] [RequireInterface(typeof(IBullet))] Object _bullet;


    [SerializeField] UnityEvent<AimDirection, Vector2> _OnShoot;


    private List<ShootPoint> _points;

    public IBullet currentBullet => (IBullet)_bullet;

    public UnityEvent<AimDirection, Vector2> OnShoot { get => _OnShoot; set => _OnShoot = value; }


    public UnityEvent<int> BulletChanged { get => new UnityEvent<int>(); set { } }


    public float reloadInterval = 1;
    public int maxBullets = 4;

    private int _currentBullets = 0;
    private float _currentReloadTime;

    public override void ServerStart()
    {
        base.MyAwake();

        _points = GetComponentsInChildren<ShootPoint>().ToList();

        Reload();
    }

    public override void ServerUpdate()
    {
        if(_currentBullets == 0)
        {
            _currentReloadTime += Time.deltaTime;
        }
        else
        {
            _currentReloadTime = 0;
        }
        if (_currentReloadTime > reloadInterval)
        {
            Reload();
        }

    }

    public bool Shoot(AimDirection direction)
    {
        return Shoot(direction, new Vector2(1, 0));
    }

    public bool Shoot(AimDirection direction, Vector2 axis)
    {
        if (_currentBullets > 0)
        {
            _currentBullets--;
            OnShoot?.Invoke(direction, axis);

            return true;
        }
        return false;
    }

    public void AssignBullet(int index)
    {
        BulletChanged?.Invoke(index);
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
        _currentBullets = maxBullets;
    }
}
