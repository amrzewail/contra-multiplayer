using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Turret : NetworkBehaviourOwner
{
    private enum Direction
    {
        Up = 0,
        RightHigh = 1,
        RightMidHigh = 2,
        Right = 3,
        RightMidLow = 4,
        RightLow = 5,
        Down = 6,
        LeftLow = 7,
        LeftMidLow = 8,
        Left = 9,
        LeftMidHigh = 10,
        LeftHigh = 11,
        Count
    }

    private enum State
    {
        Closing,
        Closed,
        Opening,
        Aiming,
        Destroyed
    }

    [SerializeField] int health = 5;
    [SerializeField] float attackRange = 5;
    [SerializeField] float turretRotateInterval = 0.5f;
    [SerializeField] int shootInterval = 2;
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
    [SyncVar] private int _currentHealth;
    private float _lastTargetUpdateTime = -5000;
    private float _lastRotateTime = 0;
    private float _lastShootTime = 0;

    public override void ServerStart()
    {
        _state = State.Closed;
        _currentHealth = health;
        _targeter = GetComponentInChildren<PlayerTargeter>();
    }

    public override void ServerUpdate()
    {
        UpdateState();

        UpdateAnimations();

        if (hitbox.IsHit(out int x))
        {
            CmdHit(x);
        }
    }

    public override void ClientUpdate()
    {
        if (hitbox.IsHit(out int x))
        {
            CmdHit(x);
        }
    }

    private void UpdateState()
    {
        switch (_state)
        {
            case State.Closed:
                _targeter.UpdateTarget(1, attackRange);
                if (_targeter.target)
                {
                    _state = State.Opening;
                }
                break;

            case State.Opening:
                if (animator.IsAnimationFinished())
                {
                    _targeter.UpdateTarget(1, attackRange);
                    UpdateDirection();
                    _currentDirection = _targetDirection;
                    _state = State.Aiming;
                }
                break;

            case State.Aiming:
                _targeter.UpdateTarget(1, attackRange);
                UpdateDirection();
                hitbox.isInvincible = false;

                if (!_targeter.target)
                {
                    _state = State.Closing;
                }
                else
                {
                    Shoot();
                }

                break;

            case State.Closing:
                hitbox.isInvincible = true;
                if (animator.IsAnimationFinished())
                {
                    _state = State.Closed;
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
        angle = 90 - angle;
        if (angle < 0) angle += 360;

        angle = angle / (360f / ((int)Direction.Count + 1));
        int x = (int)angle % (int)Direction.Count;

        _targetDirection = (Direction)x;

        if(_targetDirection != _currentDirection && Time.time - _lastRotateTime >= turretRotateInterval)
        {
            //_currentDirection = _targetDirection;
            if((int)_currentDirection < (int)_targetDirection)
            {
                if(((int)_targetDirection - (int)_currentDirection) < ((int)_currentDirection + (int)Direction.Count - (int)_targetDirection))
                {
                    _currentDirection = (Direction)((int)_currentDirection + 1);
                }
                else
                {
                    _currentDirection = _currentDirection == 0 ? _currentDirection = (Direction)((int)Direction.Count - 1) : (Direction)((int)_currentDirection - 1);
                }
            }
            else
            {
                if (((int)_currentDirection - (int)_targetDirection) < ((int)_targetDirection + (int)Direction.Count - (int)_currentDirection))
                {
                    _currentDirection = (Direction)((int)_currentDirection - 1);
                }
                else
                {
                    _currentDirection = (Direction)(((int)_currentDirection + 1) % (int)Direction.Count);
                }
            }
            _lastRotateTime = Time.time;
        }

    }

    private void Shoot()
    {
        if(Time.time - _lastShootTime > shootInterval)
        {
            shooter.Shoot((AimDirection)_currentDirection);
            _lastShootTime = Time.time;
        }
    }

    private void UpdateAnimations()
    {
        string animation = "Closed";

        switch (_state)
        {
            case State.Closed:
                animation = "Closed";
                break;

            case State.Opening:
                animation = "Opening";
                break;

            case State.Aiming:
                animation = _currentDirection.ToString();
                break;

            case State.Closing:
                animation = "Closing";
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
        _currentHealth-=hits;
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
