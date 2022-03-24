using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private PlayerTargeter _targeter;
    private float _lastTargetUpdateTime = -5000;
    private float _lastRotateTime = 0;
    private float _lastBulletTime = 0;
    private float _lastShootTime = 0;
    private int _currentBullets = 0;

    [SyncVar] private float _currentHealth;


    public override void ServerStart()
    {
        _state = State.Inactive;
        _currentHealth = health;
        _currentBullets = maxBulletCount;
        _targeter = GetComponentInChildren<PlayerTargeter>();
    }

    public override void ServerUpdate()
    {
        UpdateState();

        UpdateAnimations();

        int x;
        if (hitbox.IsHit(out x))
        {
            CmdHit(x);
        }
    }

    public override void ClientUpdate()
    {
        int x;
        if (hitbox.IsHit(out x))
        {
            CmdHit(x);
        }
    }

    private void UpdateState()
    {
        switch (_state)
        {
            case State.Inactive:
                _targeter.UpdateTarget(1, attackRange);
                if (_targeter.target)
                {
                    _state = State.Appearing;
                }
                break;

            case State.Appearing:
                if (animator.IsAnimationFinished())
                {
                    _targeter.UpdateTarget(1, attackRange);
                    UpdateDirection();
                    _currentDirection = _targetDirection;
                    _state = State.Shooting;
                }
                break;

            case State.Shooting:
                _targeter.UpdateTarget(1, attackRange);
                UpdateDirection();
                hitbox.isInvincible = false;
                if (_targeter.target)
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

    private void UpdateDirection()
    {
        if (!_targeter.target) return;

        Vector2 axis = (_targeter.target.transform.position - transform.position).normalized;
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
    private void CmdHit(int hits)
    {
        SoundEvents.Play(SFX.EnemyHit);

        _currentHealth -= (float)hits / GameNetworkManager.singleton.numberOfPlayers;
        if (_currentHealth <= 0)
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
