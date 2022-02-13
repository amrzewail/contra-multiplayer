using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SoldierHiding : NetworkBehaviourOwner
{
    [SerializeField] LookDirection lookDirection = LookDirection.Left;
    [SerializeField] float attackRange = 5;
    [SerializeField] float hidingTime = 3;
    [SerializeField] float aimingTime = 3;
    [SerializeField] float hidingYOffset = 0.5f;
    [SerializeField] [RequireInterface(typeof(IAnimator))] Object _animator;
    [SerializeField] [RequireInterface(typeof(IJumper))] Object _jumper;
    [SerializeField] [RequireInterface(typeof(IHitbox))] Object _hitbox;
    [SerializeField] [RequireInterface(typeof(IShooter))] Object _shooter;

    private State _state = State.Aiming;
    private AimDirection _aimDirection = AimDirection.Straight;
    private PlayerTargeter _targeter;

    private float _lastTargetUpdateTime = -5000;
    private float _hidingStartedTime = 0;
    private float _aimingStartedTime = 0;
    private float _startYPosition;

    public IAnimator animator => (IAnimator)_animator;
    public IJumper jumper => (IJumper)_jumper;

    public IHitbox hitbox => (IHitbox)_hitbox;
    public IShooter shooter => (IShooter)_shooter;

    private enum State
    {
        Aiming,
        Shooting,
        Hiding,
        Dead,
        Empty
    };


    public override void ServerStart()
    {
        Physics2D.IgnoreLayerCollision((int)Layer.Character, (int)Layer.Character);
        _startYPosition = transform.position.y;
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        _targeter = GetComponentInChildren<PlayerTargeter>();
    }

    public override void ServerUpdate()
    {


        UpdateState();

        UpdateAnimations();

        if (hitbox.IsHit(out int _))
        {
            Die();
        }
    }

    public override void ClientUpdate()
    {
        if (hitbox.IsHit(out int _))
        {
            DisableHitboxes();
            CmdDisableHitboxes();
        }
    }


    private void UpdateDirection()
    {
        _aimDirection = AimDirection.Straight;
        if (_targeter.target)
        {
            Vector2 axis = (_targeter.target.transform.position - transform.position).normalized;

            if (axis.x > 0) lookDirection = LookDirection.Right;
            else lookDirection = LookDirection.Left;
            transform.localScale = new Vector3(lookDirection == LookDirection.Right ? 1 : -1, 1, 1);
        }

    }

    private bool Shoot()
    {
        if (!_targeter.target) return false;

        var axis = (_targeter.target.transform.position - transform.position);
        axis.y = 0;
        if (shooter.Shoot(_aimDirection, axis))
        {
            return true;
        }
        return false;
    }

    private void UpdateState()
    {
        switch (_state)
        {
            case State.Hiding:
                Vector3 position = transform.position;
                position.y = Mathf.MoveTowards(position.y, _startYPosition - hidingYOffset, Time.deltaTime * 4);
                transform.position = position;

                if(Time.time - _hidingStartedTime > hidingTime)
                {
                    _state = State.Aiming;
                    _aimingStartedTime = Time.time;
                }
                break;

            case State.Aiming:
                position = transform.position;
                position.y = Mathf.MoveTowards(position.y, _startYPosition, Time.deltaTime * 4);
                transform.position = position;

                _targeter.UpdateTarget(5, attackRange);

                UpdateDirection();

                if(Mathf.Abs(position.y - _startYPosition) < 0.01f)
                {
                    hitbox.isInvincible = false;
                }
                if (Time.time - _aimingStartedTime > (aimingTime / 3f))
                {
                    if (Shoot())
                    {
                        _state = State.Shooting;
                    }
                    else if(Time.time - _aimingStartedTime > aimingTime)
                    {
                        _state = State.Hiding;
                        hitbox.isInvincible = true;
                        _hidingStartedTime = Time.time;
                    }
                }
                break;

            case State.Shooting:
                if (animator.IsAnimationFinished())
                {
                    _state = State.Aiming;
                }
                break;

            case State.Dead:
                if (animator.IsAnimationFinished())
                {
                    _state = State.Empty;
                    RpcDestroy();
                }
                break;
        }
    }

    private void UpdateAnimations()
    {
        string animation = "Aim Straight";


        switch (_state)
        {
            case State.Hiding:

                animation = "Aim Low";
                break;

            case State.Aiming:

                switch (_aimDirection)
                {
                    case AimDirection.Straight:
                        animation = "Aim Straight";
                        break;
                }

                break;

            case State.Shooting:
                switch (_aimDirection)
                {
                    case AimDirection.Straight:
                        animation = "Shoot Straight";
                        break;
                }
                break;

            case State.Dead:
                animation = "Dead";
                break;
        }
        animator.Play(animation);
    }

    private void Die()
    {
        RpcDisableHitboxes();
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
        jumper.Jump();
        _state = State.Dead;
        UpdateAnimations();
    }

    [Command(requiresAuthority = false)]
    public void CmdDisableHitboxes()
    {
        Die();
    }

    [ClientRpc]
    public void RpcDisableHitboxes()
    {
        DisableHitboxes();
    }

    private void DisableHitboxes()
    {
        GetComponentInChildren<Damage>().enabled = false;
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
            transform.localScale = new Vector3(lookDirection == LookDirection.Right ? 1 : -1, 1, 1);
        }
    }
    private void OnValidate()
    {
        if (!Application.isPlaying)
        {
            transform.localScale = new Vector3(lookDirection == LookDirection.Right ? 1 : -1, 1, 1);
        }
    }
#endif
}
