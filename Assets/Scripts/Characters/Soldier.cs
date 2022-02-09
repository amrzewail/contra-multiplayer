using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldier : NetworkBehaviourOwner
{
    [SerializeField] LookDirection lookDirection = LookDirection.Left;
    [SerializeField] float attackRange = 5;
    [SerializeField] [RequireInterface(typeof(IAnimator))] Object _animator;
    [SerializeField] [RequireInterface(typeof(IJumper))] Object _jumper;
    [SerializeField] [RequireInterface(typeof(IHitbox))] Object _hitbox;
    [SerializeField] [RequireInterface(typeof(IShooter))] Object _shooter;

    private State _state = State.Aiming;
    private AimDirection _aimDirection = AimDirection.Straight;
    private Player _target;
    private float _lastTargetUpdateTime = -5000;

    public IAnimator animator => (IAnimator)_animator;
    public IJumper jumper => (IJumper)_jumper;

    public IHitbox hitbox => (IHitbox)_hitbox;
    public IShooter shooter => (IShooter)_shooter;

    private enum State
    {
        Aiming,
        Shooting,
        Dead,
        Empty
    };


    public override void ServerStart()
    {
        Physics2D.IgnoreLayerCollision((int)Layer.Character, (int)Layer.Character);
    }

    public override void ServerUpdate()
    {


        UpdateState();

        UpdateAnimations();

        if (hitbox.IsHit())
        {
            jumper.Jump();
            _state = State.Dead;
            UpdateAnimations();
            RpcDisableHitboxes();
        }
    }

    public override void ClientUpdate()
    {
        if (hitbox.IsHit())
        {
            DisableHitboxes();
            CmdDisableHitboxes();
        }
    }

    private void UpdateTarget(float cooldown)
    {
        if (Time.time - _lastTargetUpdateTime < cooldown) return;

        _lastTargetUpdateTime = Time.time;
        Player[] players = Object.FindObjectsOfType<Player>();
        float dist = Vector2.Distance(transform.position, players[0].transform.position);
        int targetIndex = 0;
        for(int i = 1; i < players.Length; i++)
        {
            float d = Vector2.Distance(transform.position, players[i].transform.position);
            if(d < dist)
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
        _aimDirection = AimDirection.Straight;
        if (_target)
        {
            Vector2 axis = (_target.transform.position - transform.position).normalized;
            float angle = Mathf.Atan2(axis.y, Mathf.Abs(axis.x)) * Mathf.Rad2Deg;

            if (angle > 30) _aimDirection = AimDirection.High;
            else if (angle > -30) _aimDirection = AimDirection.Straight;
            else _aimDirection = AimDirection.Low;

            if (axis.x > 0) lookDirection = LookDirection.Right;
            else lookDirection = LookDirection.Left;

            transform.localScale = new Vector3(lookDirection == LookDirection.Right ? 1 : -1, 1, 1);

        }

    }

    private bool Shoot()
    {
        if (!_target) return false;
        if (shooter.Shoot(_aimDirection, _target.transform.position - transform.position))
        {
            return true;
        }
        return false;
    }

    private void UpdateState()
    {
        switch (_state)
        {
            case State.Aiming:
                UpdateTarget(5);

                UpdateDirection();

                if (Shoot())
                {
                    _state = State.Shooting;
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
            case State.Aiming:

                switch (_aimDirection)
                {
                    case AimDirection.High:
                        animation = "Aim High";
                        break;

                    case AimDirection.Straight:
                        animation = "Aim Straight";
                        break;

                    case AimDirection.Low:
                        animation = "Aim Low";
                        break;
                }

                break;

            case State.Shooting:
                switch (_aimDirection)
                {
                    case AimDirection.High:
                        animation = "Shoot High";
                        break;

                    case AimDirection.Straight:
                        animation = "Shoot Straight";
                        break;

                    case AimDirection.Low:
                        animation = "Shoot Low";
                        break;
                }
                break;

            case State.Dead:
                animation = "Dead";
                break;
        }
        animator.Play(animation);
    }

    [Command(requiresAuthority = false)]
    public void CmdDisableHitboxes()
    {
        RpcDisableHitboxes();

        jumper.Jump();
        _state = State.Dead;
        UpdateAnimations();
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
