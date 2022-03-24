using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SuperMachine : NetworkBehaviourOwner
{

    private enum State
    {
        Inactive,
        Shooting,
        Destroyed
    }

    private enum ShooterState
    {
        Idle,
        Shooting,
        Destroyed
    }

    private enum Part
    {
        LeftShooter,
        RightShooter,
        Core,
        Count
    }

    [SerializeField] int rightShooterHealth = 15;
    [SerializeField] int leftShooterHealth = 15;
    [SerializeField] int coreHealth = 30;

    [SerializeField] float attackRange = 15;
    [SerializeField] int maxBulletCount = 3;
    [SerializeField] float shootInterval = 0.5f;


    [Header("Animators")]
    [SerializeField] [RequireInterface(typeof(IAnimator))] Object _rightShooterAnimator;
    [SerializeField] [RequireInterface(typeof(IAnimator))] Object _leftShooterAnimator;
    [SerializeField] [RequireInterface(typeof(IAnimator))] Object _coreAnimator;

    [Header("Hitboxes")]
    [SerializeField] [RequireInterface(typeof(IHitbox))] Object _rightShooterHitbox;
    [SerializeField] [RequireInterface(typeof(IHitbox))] Object _leftShooterHitbox;
    [SerializeField] [RequireInterface(typeof(IHitbox))] Object _coreHitbox;

    [Header("Shooters")]
    [SerializeField] [RequireInterface(typeof(IShooter))] Object _rightShooter;
    [SerializeField] [RequireInterface(typeof(IShooter))] Object _leftShooter;


    private IAnimator GetAnimator(Part part)
    {
        switch (part)
        {
            case Part.LeftShooter: return (IAnimator)_leftShooterAnimator;
            case Part.RightShooter: return (IAnimator)_rightShooterAnimator;
            case Part.Core: return (IAnimator)_coreAnimator;
        }
        return null;
    }
    private IHitbox GetHitbox(Part part)
    {
        switch (part)
        {
            case Part.LeftShooter: return (IHitbox)_leftShooterHitbox;
            case Part.RightShooter: return (IHitbox)_rightShooterHitbox;
            case Part.Core: return (IHitbox)_coreHitbox;
        }
        return null;
    }
    private IShooter GetShooter(Part part)
    {
        switch (part)
        {
            case Part.LeftShooter: return (IShooter)_leftShooter;
            case Part.RightShooter: return (IShooter)_rightShooter;
        }
        return null;
    }


    private State _state;
    private ShooterState _rightShooterState;
    private ShooterState _leftShooterState;

    private PlayerTargeter _targeter;

    [SyncVar] private float _currentCoreHealth;
    [SyncVar] private float _currentRightShooterHealth;
    [SyncVar] private float _currentLeftShooterHealth;

    private float _lastTargetUpdateTime = -5000;
    private float _lastShootTime = 0;
    private int _currentShooter = 0;
    private int _currentBullets = 0;

    public override void ServerStart()
    {
        _state = State.Inactive;
        _currentCoreHealth = coreHealth;
        _currentLeftShooterHealth = leftShooterHealth;
        _currentRightShooterHealth = rightShooterHealth;

        _currentBullets = maxBulletCount;
        _targeter = GetComponentInChildren<PlayerTargeter>();
    }

    public override void ServerUpdate()
    {
        UpdateState();

        UpdateAnimations();

        for(int i = 0; i < (int)Part.Count; i++)
        {
            if (GetHitbox((Part)i).IsHit(out int x))
            {
                CmdHit((Part)i, x);
            }
        }
    }

    public override void ClientUpdate()
    {
        for (int i = 0; i < (int)Part.Count; i++)
        {
            if (GetHitbox((Part)i).IsHit(out int x))
            {
                CmdHit((Part)i, x);
            }
        }
    }

    private void UpdateState()
    {
        switch (_state)
        {
            case State.Inactive:
                _targeter.UpdateTarget(2, attackRange);
                if (_targeter.target)
                {
                    RpcInvokeBossStarted();
                    _state = State.Shooting;
                }
                break;

            case State.Shooting:
                Part shootingPart;
                if(Shoot(out shootingPart))
                {
                    switch (shootingPart)
                    {
                        case Part.RightShooter: _rightShooterState = ShooterState.Shooting; break;
                        case Part.LeftShooter: _leftShooterState = ShooterState.Shooting; break;
                    }
                }
                break;

            case State.Destroyed:

                break;
        }

        if (_currentRightShooterHealth <= 0) _rightShooterState = ShooterState.Destroyed;
        switch (_rightShooterState)
        {
            case ShooterState.Shooting:
                if (GetAnimator(Part.RightShooter).IsAnimationFinished())
                {
                    _rightShooterState = ShooterState.Idle;
                }
                break;
        }

        if (_currentLeftShooterHealth <= 0) _leftShooterState = ShooterState.Destroyed;
        switch (_leftShooterState)
        {
            case ShooterState.Shooting:
                if (GetAnimator(Part.LeftShooter).IsAnimationFinished())
                {
                    _leftShooterState = ShooterState.Idle;
                }
                break;
        }
    }


    private void UpdateDirection()
    {
        if (!_targeter.target)
            return;

    }

    private bool Shoot(out Part shootingPart)
    {
        if(Time.time - _lastShootTime > shootInterval)
        {
            _lastShootTime = Time.time;

            _currentShooter++;
            if(_currentShooter % 2 == 0)
            {
                if (_leftShooterState != ShooterState.Destroyed)
                {
                    shootingPart = Part.LeftShooter;
                    return GetShooter(Part.LeftShooter).Shoot(AimDirection.Straight);
                }
            }
            else
            {
                if (_rightShooterState != ShooterState.Destroyed)
                {
                    shootingPart = Part.RightShooter;
                    return GetShooter(Part.RightShooter).Shoot(AimDirection.Straight);
                }
            }
        }
        shootingPart = Part.Count;
        return false;
    }

    private void UpdateAnimations()
    {
        switch (_rightShooterState)
        {
            case ShooterState.Idle: GetAnimator(Part.RightShooter).Play("Idle"); break;
            case ShooterState.Shooting: GetAnimator(Part.RightShooter).Play("Shoot"); break;
            case ShooterState.Destroyed: GetAnimator(Part.RightShooter).Play("Destroy"); break;
        }
        switch (_leftShooterState)
        {
            case ShooterState.Idle: GetAnimator(Part.LeftShooter).Play("Idle"); break;
            case ShooterState.Shooting: GetAnimator(Part.LeftShooter).Play("Shoot"); break;
            case ShooterState.Destroyed: GetAnimator(Part.LeftShooter).Play("Destroy"); break;
        }
        switch (_state)
        {
            case State.Destroyed:
                GetAnimator(Part.Core).Play("Destroy");
                break;
        }
    }

    private void Die(Part part)
    {
        GetHitbox(part).isInvincible = true;
        switch (part)
        {
            case Part.LeftShooter:
                _leftShooterState = ShooterState.Destroyed;
                _currentLeftShooterHealth = 0;
                break;
            case Part.RightShooter:
                _rightShooterState = ShooterState.Destroyed;
                _currentRightShooterHealth = 0;
                break;
            case Part.Core:
                _state = State.Destroyed;
                RpcDisableHitboxes();
                break;
        }
        UpdateAnimations();
    }

    [Command(requiresAuthority = false)]
    private void CmdHit(Part part, int hits)
    {
        switch (part)
        {
            case Part.LeftShooter:
                _currentLeftShooterHealth -= (float)hits / GameNetworkManager.singleton.numberOfPlayers;
                if (_currentLeftShooterHealth <= 0)
                {
                    Die(part);
                }
                break;
            case Part.RightShooter:
                _currentRightShooterHealth -= (float)hits / GameNetworkManager.singleton.numberOfPlayers;
                if (_currentRightShooterHealth <= 0)
                {
                    Die(part);
                }
                break;
            case Part.Core:
                _currentCoreHealth -= (float)hits / GameNetworkManager.singleton.numberOfPlayers;
                if (_currentCoreHealth <= 0)
                {
                    RpcInvokeBossDefeated();
                    Die(Part.Core);
                    if (_leftShooterState != ShooterState.Destroyed) Die(Part.LeftShooter);
                    if (_rightShooterState != ShooterState.Destroyed) Die(Part.RightShooter);
                }
                break;
        }

    }

    [ClientRpc]
    public void RpcInvokeBossDefeated()
    {
        LevelEvents.StageBossDefeated?.Invoke();
    }
    [ClientRpc]
    public void RpcInvokeBossStarted()
    {
        LevelEvents.StageBossStarted?.Invoke();
    }

    [ClientRpc]
    public void RpcDisableHitboxes()
    {
        DisableHitboxes();
    }

    private void DisableHitboxes()
    {
        //GetComponentInChildren<Rigidbody2D>().isKinematic = true;
        for (int i = 0; i < (int)Part.Count; i++)
        {
            GetHitbox((Part)i).isInvincible = true;
        }
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
