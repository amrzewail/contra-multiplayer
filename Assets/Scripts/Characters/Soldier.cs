using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private PlayerTargeter _targeter;
    private float _lastTargetUpdateTime = -5000;

    [SyncVar] private float _currentHealth;

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

    public override void ClientStart()
    {
        GetComponent<Collider2D>().isTrigger = true;
        GetComponent<Rigidbody2D>().isKinematic = true;
    }

    public override void ServerStart()
    {
        Physics2D.IgnoreLayerCollision((int)Layer.Character, (int)Layer.Character);
        _currentHealth = 1;
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

    private void UpdateDirection()
    {
        _aimDirection = AimDirection.Straight;
        if (_targeter.target)
        {
            Vector2 axis = (_targeter.target.transform.position - transform.position).normalized;
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
        if (!_targeter.target) return false;

        var axis = (_targeter.target.transform.position - transform.position);
        if (_aimDirection == AimDirection.Low) axis += Vector3.down * 0.5f;
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
            case State.Aiming:
                _targeter.UpdateTarget(1, attackRange);

                UpdateDirection();

                if (Shoot())
                {
                    _state = State.Shooting;
                    SoundEvents.Play(SFX.EnemyGun);
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

    private void Die()
    {
        RpcDisableHitboxes();
        GetComponent<Rigidbody2D>().gravityScale = 1f;
        jumper.Jump();
        _state = State.Dead;
        UpdateAnimations();
    }

    [Command(requiresAuthority = false)]
    private void CmdHit(int hits)
    {
        SoundEvents.Play(SFX.EnemyHit2);

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
