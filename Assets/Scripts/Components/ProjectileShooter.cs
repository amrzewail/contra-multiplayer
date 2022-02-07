using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class ProjectileShooter : MonoBehaviourOwner, IShooter
{

    [SerializeField] [RequireInterface(typeof(IBullet))] Object _currentBullet;
    [SerializeField] [RequireInterface(typeof(IBullet))] Object[] _bullets;


    [SerializeField] UnityEvent<AimDirection> _OnShoot;
    [SerializeField] UnityEvent<int> _BulletChanged;


    private List<ShootPoint> _points;

    public IBullet currentBullet => (IBullet)_currentBullet;

    public UnityEvent<AimDirection> OnShoot { get => _OnShoot; set => _OnShoot = value; }
    public UnityEvent<int> BulletChanged { get => _BulletChanged; set => _BulletChanged = value; }


    public override void MyStart()
    {
        base.MyAwake();

        _points = GetComponentsInChildren<ShootPoint>().ToList();
    }

    public bool Shoot(AimDirection direction)
    {
        //GameObject g = Instantiate(bullet.gameObject, point.transform.position, point.transform.rotation);

        OnShoot?.Invoke(direction);

        return true;
    }

    public void AssignBullet(int index)
    {
        _currentBullet = _bullets.Single(x => ((IBullet)x).index == index);
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
}
