using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundTurret : NetworkBehaviourOwner
{
    private enum Direction
    {
        Up = 0,
        High = 1,
        Straight = 2,
        Count
    }

    private enum State
    {
        Inactive,
        Appearing,
        Shooting,
        Destroyed
    }

    [SerializeField] int health = 5;
    [SerializeField] float attackRange = 5;
    [SerializeField] float turretRotateInterval = 0.5f;
    [SerializeField] int maxBulletCount = 3;
    [SerializeField] float bulletInterval = 0.5f;
    [SerializeField] float shootInterval = 2;
    [SerializeField] [RequireInterface(typeof(IAnimator))] Object _animator;
    [SerializeField] [RequireInterface(typeof(IHitbox))] Object _hitbox;
    [SerializeField] [RequireInterface(typeof(IShooter))] Object _shooter;


    public IAnimator animator => (IAnimator)_animator;
    public IHitbox hitbox => (IHitbox)_hitbox;
    public IShooter shooter => (IShooter)_shooter;


    private State _state;
    private Direction _currentDirection;
    private Direction _targetDirection;
    private Player _target;
    [SyncVar] private int _currentHealth;
    private float _lastTargetUpdateTime = -5000;
    private float _lastRotateTime = 0;
    private float _lastBulletTime = 0;
    private float _lastShootTime = 0;
    private int _currentBullets = 0;

    public override void ServerStart()
    {
        _state = State.Inactive;
        _currentHealth = health;
        _currentBullets = maxBulletCount;
    }

    public override void ServerUpdate()
    {
        UpdateState();

        UpdateAnimations();

        if (hitbox.IsHit())
        {
            CmdHit();
        }
    }

    public override void ClientUpdate()
    {
        if (hitbox.IsHit())
        {
            CmdHit();
        }
    }

    private void UpdateState()
    {
        switch (_state)
        {
            case State.Inactive:
                UpdateTarget(1);
                if (_target)
                {
                    _state = State.Appearing;
                }
                break;

            case State.Appearing:
                if (animator.IsAnimationFinished())
                {
                    UpdateTarget(1);
                    UpdateDirection();
                    _currentDirection = _targetDirection;
                    _state = State.Shooting;
                }
                break;

            case State.Shooting:
                UpdateTarget(1);
                UpdateDirection();
                hitbox.isInvincible = false;
                if (_target)
                {
                    Shoot();
                }
                break;

            case State.Destroyed:
                if (animator.IsAnimationFinished())
                {
                    RpcDestroy();
                }
                break;
        }
    }

    private void UpdateTarget(float cooldown)
    {
        if (Time.time - _lastTargetUpdateTime < cooldown) return;

        _lastTargetUpdateTime = Time.time;
        Player[] players = Object.FindObjectsOfType<Player>();
        float dist = Vector2.Distance(transform.position, players[0].transform.position);
        int targetIndex = 0;
        for (int i = 1; i < players.Length; i++)
        {
            float d = Vector2.Distance(transform.position, players[i].transform.position);
            if (d < dist)
            {
                dist = d;
                targetIndex = i;
            }
        }
        if (dist < attackRange) _target = players[targetIndex];
        else _target = null;
    }

    private void UpdateDirection()
    {
        if (!_target) return;

        Vector2 axis = (_target.transform.position - transform.position).normalized;
        float angle = Mathf.Atan2(axis.y, axis.x) * Mathf.Rad2Deg;
        angle = 180 - angle;

        if (angle < 30) _targetDirection = Direction.Straight;
        else if (angle < 50) _targetDirection = Direction.High;
        else if (angle <= 180) _targetDirection = Direction.Up;
        else _targetDirection = Direction.Straight;


        if(_targetDirection != _currentDirection && Time.time - _lastRotateTime >= turretRotateInterval)
        {
            _currentDirection = (Direction)((int)_currentDirection + Mathf.Sign((int)_targetDirection - (int)_currentDirection));
            _lastRotateTime = Time.time;
        }

    }

    private void Shoot()
    {
        if(Time.time - _lastShootTime > shootInterval)
        {
            if(_currentBullets > 0)
            {
                if(Time.time - _lastBulletTime > bulletInterval)
                {
                    _currentBullets--;
                    shooter.Shoot((AimDirection)_currentDirection);
                    _lastBulletTime = Time.time;
                }
            }
            else
            {
                _currentBullets = maxBulletCount;
                _lastShootTime = Time.time;
            }
        }
    }

    private void UpdateAnimations()
    {
        string animation = "Closed";

        switch (_state)
        {
            case State.Inactive:
                animation = "Empty";
                break;

            case State.Appearing:
                animation = "Appear";
                break;

            case State.Shooting:
                animation = _currentDirection.ToString();
                break;

            case State.Destroyed:
                animation = "Destroy";
                break;

        }

        animator.Play(animation);
    }

    private void Die()
    {
        _state = State.Destroyed;
        RpcDisableHitboxes();
        UpdateAnimations();
    }

    [Command(requiresAuthority = false)]
    private void CmdHit()
    {
        _currentHealth--;
        if(_currentHealth <= 0)
        {
            Die();
        }
    }

    [ClientRpc]
    public void RpcDisableHitboxes()
    {
        DisableHitboxes();
    }

    private void DisableHitboxes()
    {
        //GetComponentInChildren<Rigidbody2D>().isKinematic = true;
        hitbox.isInvincible = true;
    }

    [ClientRpc]
    public void RpcDestroy()
    {
        Destroy(this.gameObject);
    }


#if UNITY_EDITOR

    private void Reset()
    {
        if (!Application.isPlaying)
        {
            //transform.localScale = new Vector3(lookDirection == LookDirection.Right ? 1 : -1, 1, 1);
        }
    }
    private void OnValidate()
    {
        if (!Application.isPlaying)
        {
            //transform.localScale = new Vector3(lookDirection == LookDirection.Right ? 1 : -1, 1, 1);
        }
    }
#endif
}
