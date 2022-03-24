using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FalconBox : NetworkBehaviourOwner
{

    private enum State
    {
        Closing,
        Closed,
        Opening,
        Opened,
        Destroyed
    }

    [SerializeField] float _openCloseTime = 3;
    [SerializeField] [RequireInterface(typeof(IAnimator))] Object _animator;
    [SerializeField] [RequireInterface(typeof(IHitbox))] Object _hitbox;
    [SerializeField] GameObject spawnObject;

    public IAnimator animator => (IAnimator)_animator;
    public IHitbox hitbox => (IHitbox)_hitbox;


    private State _state;
    [SyncVar] private float _currentHealth;
    private float _lastTransitionTime;

    public override void ServerStart()
    {
        _state = State.Closed;
        _currentHealth = 1;
        hitbox.isInvincible = true;
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
                if(Time.time - _lastTransitionTime > _openCloseTime)
                {
                    _state = State.Opening;
                    hitbox.isInvincible = false;
                }
                break;

            case State.Opening:
                if (animator.IsAnimationFinished())
                {
                    _state = State.Opened;
                    _lastTransitionTime = Time.time;
                }
                break;

            case State.Opened:
                if (Time.time - _lastTransitionTime > _openCloseTime)
                {
                    _state = State.Closing;
                }
                break;

            case State.Closing:
                if (animator.IsAnimationFinished())
                {
                    _state = State.Closed;
                    _lastTransitionTime = Time.time;
                    hitbox.isInvincible = true;
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

    private void UpdateAnimations()
    {
        string animation = "Closed";

        switch (_state)
        {
            case State.Closed:
                animation = "Closed";
                break;

            case State.Closing:
            case State.Opening:
                animation = "Transition";
                break;

            case State.Opened:
                animation = "Opened";
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
        if (_currentHealth > 0)
        {
            _currentHealth -= hits;
            if (_currentHealth <= 0)
            {
                GameObject powerup = GameObject.Instantiate(spawnObject, this.transform.position, Quaternion.identity);
                NetworkServer.Spawn(powerup);
                Die();
            }
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
