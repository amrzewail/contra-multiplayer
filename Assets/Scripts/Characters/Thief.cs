using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Thief : NetworkBehaviourOwner
{
    [SerializeField] LookDirection lookDirection = LookDirection.Left;
    [SerializeField] float moveRange = 15;
    [SerializeField] [RequireInterface(typeof(IAnimator))] Object _animator;
    [SerializeField] [RequireInterface(typeof(IMover))] Object _mover;
    [SerializeField] [RequireInterface(typeof(IJumper))] Object _jumper;
    [SerializeField] [RequireInterface(typeof(IGrounder))] Object _grounder;
    [SerializeField] [RequireInterface(typeof(IHitbox))] Object _hitbox;

    private State _state = State.Jumping;

    public IAnimator animator => (IAnimator)_animator;
    public IMover mover => (IMover)_mover;
    public IJumper jumper => (IJumper)_jumper;

    public IGrounder grounder => (IGrounder)_grounder;
    public IHitbox hitbox => (IHitbox)_hitbox;

    private PlayerTargeter _targeter;
    private float _lastTargetUpdateTime;

    private enum State
    {
        Moving,
        Jumping,
        Dead,
        Splash,
        Empty
    };


    public override void ServerStart()
    {
        Physics2D.IgnoreLayerCollision((int)Layer.Character, (int)Layer.Character);
        _targeter = GetComponentInChildren<PlayerTargeter>();
    }

    public override void ServerUpdate()
    {
        UpdateLookDirection();

        UpdateState();

        UpdateAnimations();

        if (hitbox.IsHit(out int _))
        {
            jumper.Jump();
            _state = State.Dead;
            UpdateAnimations();
            RpcDisableHitboxes();
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

    private void UpdateLookDirection()
    {
        transform.localScale = new Vector3(lookDirection == LookDirection.Right ? 1 : -1, 1, 1);

    }



    private void UpdateState()
    {
        Vector3 moveDirection = new Vector2(lookDirection == LookDirection.Right ? 1 : -1, 0);


        switch (_state)
        {
            case State.Moving:
                _targeter.UpdateTarget(0.5f, moveRange);
                if (_targeter.target)
                {
                    if (!grounder.IsGrounded())
                    {
                        int random = UnityEngine.Random.Range(0, 11);
                        if (random < 3)
                        {
                            lookDirection = (lookDirection == LookDirection.Right ? LookDirection.Left : LookDirection.Right);
                            moveDirection = new Vector2(lookDirection == LookDirection.Right ? 1 : -1, 0);
                            mover.Move(moveDirection);
                            transform.position += moveDirection * 0.2f;
                        }
                        else
                        {
                            jumper.Jump();
                            _state = State.Jumping;
                        }
                    }
                    else
                    {
                        mover.Move(moveDirection);
                        if (grounder.HasGroundLayer(Layer.Water))
                        {
                            _state = State.Splash;
                            RpcDisableHitboxes();
                        }
                    }
                }
                break;

            case State.Jumping:
                if (grounder.IsGrounded())
                {
                    if (grounder.HasGroundLayer(Layer.Water))
                    {
                        _state = State.Splash;
                        RpcDisableHitboxes();
                    }
                    else
                    {
                        _state = State.Moving;
                    }
                }
                break;
            case State.Splash:
            case State.Dead:
                mover.Move(Vector2.zero);
                if (animator.IsAnimationFinished())
                {
                    _state = State.Empty;
                    RpcDestroy();
                }
                else
                {
                    if(_state == State.Dead)
                    {
                        mover.Move(new Vector2(lookDirection == LookDirection.Right ? -1 : 1, 0));
                    }
                }
                break;
        }
    }

    private void UpdateAnimations()
    {
        string animation = "Run";
        switch (_state)
        {
            case State.Moving:
                animation = "Run";
                break;

            case State.Jumping:
                animation = "Jump";
                break;

            case State.Splash:
                animation = "Splash";
                break;

            case State.Empty:
                animation = "Empty";
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
