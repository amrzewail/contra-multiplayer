using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public enum LookDirection
{
    Right,
    Left
}

public class Player : MonoBehaviourOwner
{


    public enum State
    {
        Idle,
        Moving,
        Jumping,
        Falling,
        Splash,
        Dive,
        Swimming,
        WaterGetUp,
        Dead,
        Spectate
    }

    [SerializeField] int _maxLives = 3;
    [SerializeField] [RequireInterface(typeof(IAnimator))] Object _bodyAnimator;
    [SerializeField] [RequireInterface(typeof(IAnimator))] Object _legsAnimator;

    [SerializeField] [RequireInterface(typeof(IInput))] Object _input;
    [SerializeField] [RequireInterface(typeof(IGrounder))] Object _grounder;
    [SerializeField] [RequireInterface(typeof(IMover))] Object _mover;
    [SerializeField] [RequireInterface(typeof(IJumper))] Object _jumper;
    [SerializeField] [RequireInterface(typeof(IShooter))] Object _shooter;
    [SerializeField] [RequireInterface(typeof(IHitbox))] Object _hitbox;

    public IAnimator bodyAnimator => (IAnimator)_bodyAnimator;
    public IAnimator legsAnimator => (IAnimator)_legsAnimator;

    public IInput input => (IInput)_input;
    public IGrounder grounder => (IGrounder)_grounder;

    public IMover mover => (IMover)_mover;
    public IJumper jumper => (IJumper)_jumper;

    public IShooter shooter => (IShooter)_shooter;

    public IHitbox hitbox => (IHitbox)_hitbox;

    [HideInInspector] public int lives = 3;

    public bool isInvader { get; set; }

    private Collider2D _collider;
    private Rigidbody2D _rigidBody;
    private Invincibility _invincibility;

    private bool _isShooting = false;
    private Vector2 _lastVelocity;
    private Vector3 _lastPosition;
    private float _lastShootingTime;
    private float _startFallTime;
    private float _deadTime;

    private State _state;
    private AimDirection _aimDirection;
    private LookDirection _lookDirection;

    public State state => _state;

    public override void MyStart()
    {
        lives = _maxLives;

        Physics2D.IgnoreLayerCollision((int)Layer.Player, (int)Layer.Player);
        Physics2D.IgnoreLayerCollision((int)Layer.Player, (int)Layer.Character);
        _collider = GetComponent<Collider2D>();
        _rigidBody = GetComponent<Rigidbody2D>();
        _invincibility = GetComponentInChildren<Invincibility>();

        _state = State.Jumping;
    }

    public override void OtherStart()
    {
        _collider = GetComponent<Collider2D>();
        _rigidBody = GetComponent<Rigidbody2D>();

        _collider.isTrigger = true;
        _rigidBody.isKinematic = true;

        hitbox.gameObject.SetActive(false);
    }

    public override void MyUpdate()
    {

        float horizontal = input.GetHorizontal();
        float vertical = input.GetVertical();

        UpdateState();

        UpdateLookDirection();

        UpdateAimDirection();

        UpdateAnimations();

        UpdateHitbox();
    }

    private void Respawn()
    {
        Camera camera = FindObjectOfType<Camera>();
        Vector3 position = transform.position;
        position.y = camera.transform.position.y + camera.orthographicSize;
        transform.position = position;
        _rigidBody.velocity = Vector2.zero;
        shooter.AssignBullet(0);
        shooter.ResetFireRate();
        _state = State.Jumping;
        _invincibility.Trigger();
    }

    private void Shoot()
    {
        if (shooter.Shoot(_aimDirection))
        {
            SoundEvents.Play(SFX.EnemyGun);

            _isShooting = true;
            _lastShootingTime = Time.time;
        }
    }

    private void UpdateState()
    {
        float horizontal = input.GetHorizontal();
        float vertical = input.GetVertical();
        bool shoot = shooter.GetBullet().isContinuous ? input.ShootHold() : input.ShootDown();
        bool jump = input.Jump();


        switch (_state)
        {
            case State.Idle:

                if (Mathf.Abs(horizontal) > 0.05f) _state = State.Moving;
                if (!grounder.IsGrounded()) _state = State.Falling;
                if (jump)
                {
                    if(_aimDirection == AimDirection.Down)
                    {
                        if (grounder.GetGroundLayer() == Layer.Platform && !grounder.HasGroundLayer(Layer.Ground))
                        {
                            _state = State.Falling;
                            Physics2D.IgnoreLayerCollision(this.gameObject.layer, (int)Layer.Platform, true);
                            _startFallTime = Time.time;
                        }
                    }
                    else
                    {
                        _state = State.Jumping;
                        jumper.Jump();
                    }
                }
                if (_isShooting)
                {
                    if(bodyAnimator.IsAnimationFinished())
                    {
                        _isShooting = false;
                    }
                }
                if (grounder.GetGroundLayer() == Layer.Water && (!grounder.HasGroundLayer(Layer.Ground) && !grounder.HasGroundLayer(Layer.Platform)))
                {
                    _state = State.Splash;
                }
                mover.Move(new Vector2(0, 0));
                if (shoot) Shoot();

                break;
            case State.Moving:

                if (Mathf.Abs(horizontal) <= 0.05f) _state = State.Idle;
                if (!grounder.IsGrounded()) _state = State.Falling;
                if (jump)
                {
                    _state = State.Jumping;
                    jumper.Jump();
                }
                if (_isShooting)
                {
                    if (bodyAnimator.IsAnimationFinished())
                    {
                        _isShooting = false;
                    }
                }
                if (grounder.GetGroundLayer() == Layer.Water && (!grounder.HasGroundLayer(Layer.Ground) && !grounder.HasGroundLayer(Layer.Platform)))
                {
                    _state = State.Splash;
                }
                mover.Move(new Vector2(horizontal, 0));
                if (shoot) Shoot();

                break;
            case State.Jumping:

                if (_rigidBody.velocity.y > 0.1f)
                {
                    Physics2D.IgnoreLayerCollision(this.gameObject.layer, (int)Layer.Platform, true);
                    Physics2D.IgnoreLayerCollision(this.gameObject.layer, (int)Layer.Ground, true);
                }
                else
                {
                    if (grounder.IsGrounded())
                    {
                        if (grounder.GetGroundLayer() == Layer.Water && (!grounder.HasGroundLayer(Layer.Ground) && !grounder.HasGroundLayer(Layer.Platform)))
                        {
                            _state = State.Splash;
                        }
                        else
                        {
                            _state = State.Idle;
                        }
                    }
                    Physics2D.IgnoreLayerCollision(this.gameObject.layer, (int)Layer.Platform, false);
                    Physics2D.IgnoreLayerCollision(this.gameObject.layer, (int)Layer.Ground, false);

                }
                if (Mathf.Abs(horizontal) > 0.01f)
                {
                    mover.Move(new Vector2(horizontal, 0));
                }
                if (shoot) Shoot();

                break;
            case State.Falling:

                if (Time.time - _startFallTime > 0.3f)
                {
                    Physics2D.IgnoreLayerCollision(this.gameObject.layer, (int)Layer.Platform, false);
                    if (grounder.IsGrounded())
                    {
                        if (grounder.GetGroundLayer() == Layer.Water && (!grounder.HasGroundLayer(Layer.Ground) && !grounder.HasGroundLayer(Layer.Platform)))
                        {
                            _state = State.Splash;
                        }
                        else
                        {
                            _state = State.Idle;
                        }
                    }
                }
                if (shoot) Shoot();

                break;

            case State.Splash:
                if (legsAnimator.IsAnimationFinished())
                {
                    _state = State.Swimming;
                }
                mover.Move(new Vector2(0, 0));
                break;

            case State.Swimming:
                mover.Move(new Vector2(horizontal, 0));
                if (_isShooting)
                {
                    if (legsAnimator.IsAnimationFinished())
                    {
                        _isShooting = false;
                    }
                }
                if(vertical < -0.05f)
                {
                    _state = State.Dive;
                }
                if (shoot) Shoot();
                if(grounder.HasGroundLayer(Layer.Ground) || grounder.HasGroundLayer(Layer.Platform))
                {
                    _state = State.WaterGetUp;
                }
                break;

            case State.Dive:
                if (vertical > -0.05f)
                {
                    _state = State.Swimming;
                    hitbox.isInvincible = false;
                }
                mover.Move(new Vector2(0, 0));
                hitbox.isInvincible = true;
                break;
            case State.WaterGetUp:
                if (bodyAnimator.IsAnimationFinished())
                {
                    _state = State.Idle;
                }
                mover.Move(new Vector2(0, 0));
                break;

            case State.Dead:
                if (!legsAnimator.IsAnimationFinished())
                {
                    mover.Move(new Vector2(_lookDirection == LookDirection.Right ? -1 : 1, 0));
                    if (_rigidBody.velocity.y > 0.1f)
                    {
                        Physics2D.IgnoreLayerCollision(this.gameObject.layer, (int)Layer.Platform, true);
                    }
                    else
                    {
                        Physics2D.IgnoreLayerCollision(this.gameObject.layer, (int)Layer.Platform, false);

                    }
                }
                else
                {
                    Physics2D.IgnoreLayerCollision(this.gameObject.layer, (int)Layer.Platform, false);
                    mover.Move(new Vector2(0, 0));
                }
                hitbox.isInvincible = true;

                if(Time.time - _deadTime > 2)
                {
                    if (lives > 0)
                    {
                        lives--;
                        Respawn();
                    }
                    else
                    {
                        _state = State.Spectate;
                        LevelController.instance.StartSpectate();
                        LevelEvents.OnPlayerDead?.Invoke(identity.netId);
                    }
                }

                break;

            case State.Spectate:
                hitbox.isInvincible = true;
                _invincibility.isInvincible = true;
                if (jump)
                {
                    LevelController.instance.ChangeSpectateTarget(1);
                }
                break;
        }


        if (hitbox.IsHit(out int _))
        {
            _state = State.Dead;
            _deadTime = Time.time;
            _rigidBody.velocity = Vector2.zero;
            jumper.Jump();

            SoundEvents.Play(SFX.Death);
        }

        _lastVelocity = _rigidBody.velocity;
        _lastPosition = transform.position;
    }

    private void UpdateHitbox()
    {
        if (_state == State.Dead) return;
        if (_state == State.Spectate) return;

        hitbox.isInvincible = _invincibility.isInvincible;
        switch (_state)
        {
            case State.Idle:
            case State.Moving:
                if (_aimDirection == AimDirection.Down)
                {
                    hitbox.SetVOffset(-0.625f);
                    hitbox.SetVSize(0.75f);
                }
                else
                {
                    hitbox.SetVOffset(0);
                    hitbox.SetVSize(2);
                }
                break;
            case State.Swimming:
            case State.WaterGetUp:
                hitbox.SetVOffset(-0.5f);
                hitbox.SetVSize(1);
                break;
            case State.Jumping:
                hitbox.SetVOffset(0.5f);
                hitbox.SetVSize(1);
                break;

            case State.Falling:
                hitbox.SetVOffset(0);
                hitbox.SetVSize(2);
                break;

            case State.Splash:
            case State.Dive:
                hitbox.SetVOffset(0);
                hitbox.SetVSize(2);
                hitbox.isInvincible = true;
                break;

        }
    }

    private void UpdateLookDirection()
    {
        float horizontal = input.GetHorizontal();
        if (horizontal > 0) _lookDirection = LookDirection.Right;
        else if (horizontal < 0) _lookDirection = LookDirection.Left;
    }

    private void UpdateAimDirection()
    {
        float horizontal = input.GetHorizontal();
        float vertical = input.GetVertical();

        int x = 0;// _lookDirection == LookDirection.Right ? 0 : 5;

        float angle = Mathf.Atan2(vertical, Mathf.Abs(horizontal)) * Mathf.Rad2Deg;

        x += (int)((90 - angle) / 45);

        _aimDirection = ((AimDirection)x);

        if(_state == State.Jumping)
        {
            if (_aimDirection == AimDirection.Up) _aimDirection = AimDirection.StraightUp;
            else if (_aimDirection == AimDirection.Down) _aimDirection = AimDirection.StraightDown;
        }

        if(_state == State.Falling)
        {
            _aimDirection = AimDirection.Straight;
        }

        if(_state == State.Swimming)
        {
            switch (_aimDirection)
            {
                case AimDirection.Straight: _aimDirection = AimDirection.WaterStraight; break;
                case AimDirection.High: _aimDirection = AimDirection.WaterHigh; break;
                case AimDirection.Up: _aimDirection = AimDirection.WaterUp; break;

            }
        }

    }
    private void UpdateAnimations()
    {
        string legsAnimation = "Idle";
        string bodyAnimation = "Idle";

        bool ignoreBodyAnimation = false;
        bool ignoreLegsAnimation = false;

        if (_state == State.Idle || _state == State.Moving)
        {

            if (_state == State.Idle)
            {
                bodyAnimation = "Aim Straight";
                legsAnimation = "Idle";
            }
            else if(_state == State.Moving)
            {
                bodyAnimation = "Run";
                legsAnimation = "Run";

                transform.localScale = new Vector3(_lookDirection == LookDirection.Right ? 1 : -1, 1, 1);
            }

            switch (_aimDirection)
            {
                case AimDirection.Up:
                    bodyAnimation = "Aim Up";
                    break;
                case AimDirection.Down:
                    legsAnimation = "Aim Down";
                    bodyAnimation = "Aim Down";
                    break;
                case AimDirection.High:
                    bodyAnimation = "Aim High";
                    break;
                case AimDirection.Low:
                    bodyAnimation = "Aim Low";
                    break;
            }


            if (_isShooting)
            {
                bodyAnimation = bodyAnimation.Replace("Aim", "Shoot");
                bodyAnimation = bodyAnimation.Replace("Run", "Shoot Straight");

                if (legsAnimation.Contains("Aim"))
                {
                    legsAnimation = legsAnimation.Replace("Aim", "Shoot"); 
                    ignoreLegsAnimation = Time.time - _lastShootingTime < Time.deltaTime;

                }
                if(Time.time - _lastShootingTime < Time.deltaTime)
                {
                    ignoreBodyAnimation = Time.time - _lastShootingTime < Time.deltaTime;
                }

            }
        }else if(_state == State.Jumping)
        {
            bodyAnimation = "Jump";
            legsAnimation = "Jump";

            transform.localScale = new Vector3(_lookDirection == LookDirection.Right ? 1 : -1, 1, 1);
        }else if (_state == State.Falling)
        {
            bodyAnimation = "Falling";
            legsAnimation = "Falling";
        }else if (_state == State.Swimming)
        {
            transform.localScale = new Vector3(_lookDirection == LookDirection.Right ? 1 : -1, 1, 1);

            bodyAnimation = "Empty";
            legsAnimation = "Water Idle";
            switch (_aimDirection)
            {
                case AimDirection.WaterUp:
                    legsAnimation = "Water Aim Up";
                    break;
                case AimDirection.WaterHigh:
                    legsAnimation = "Water Aim High";
                    break;
            }
            if (_isShooting)
            {
                if(_aimDirection == AimDirection.WaterStraight)
                {
                    legsAnimation = "Water Shoot Straight";
                }
                else
                {
                    legsAnimation = legsAnimation.Replace("Aim", "Shoot");
                }

                if (Time.time - _lastShootingTime < Time.deltaTime)
                {
                    ignoreLegsAnimation = Time.time - _lastShootingTime < Time.deltaTime;
                }

            }
        }else if(_state == State.Splash)
        {
            legsAnimation = "Splash";
            bodyAnimation = "Empty";
        }else if (_state == State.Dive)
        {
            legsAnimation = "Dive";
            bodyAnimation = "Empty";
        }else if (_state == State.WaterGetUp)
        {
            legsAnimation = "Water GetUp";
            bodyAnimation = "Water GetUp";

        }else if(_state == State.Dead)
        {
            bodyAnimation = "Empty";
            legsAnimation = "Dead";
        }else if (_state == State.Spectate)
        {
            bodyAnimation = "Empty";
            legsAnimation = "Empty";
        }

        bodyAnimator.Play(bodyAnimation, ignoreBodyAnimation);
        legsAnimator.Play(legsAnimation, ignoreLegsAnimation);
    }
}
