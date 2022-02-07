using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Player : MonoBehaviourOwner
{

    private enum LookDirection
    {
        Right,
        Left
    }

    private enum State
    {
        Idle,
        Moving,
        Jumping
    }

    [SerializeField] [RequireInterface(typeof(IAnimator))] Object _bodyAnimator;
    [SerializeField] [RequireInterface(typeof(IAnimator))] Object _legsAnimator;

    [SerializeField] [RequireInterface(typeof(IInput))] Object _input;
    [SerializeField] [RequireInterface(typeof(IGrounder))] Object _grounder;
    [SerializeField] [RequireInterface(typeof(IMover))] Object _mover;
    [SerializeField] [RequireInterface(typeof(IJumper))] Object _jumper;
    [SerializeField] [RequireInterface(typeof(IShooter))] Object _shooter;

    public IAnimator bodyAnimator => (IAnimator)_bodyAnimator;
    public IAnimator legsAnimator => (IAnimator)_legsAnimator;

    public IInput input => (IInput)_input;
    public IGrounder grounder => (IGrounder)_grounder;

    public IMover mover => (IMover)_mover;
    public IJumper jumper => (IJumper)_jumper;

    public IShooter shooter => (IShooter)_shooter;

    private State _state;
    private bool _isShooting = false;
    private float _lastShootingTime;
    private AimDirection _aimDirection;
    private LookDirection _lookDirection;


    public override void MyStart()
    {
        Physics2D.IgnoreLayerCollision(6, 6);
    }

    public override void OtherStart()
    {
        Destroy(GetComponentInChildren<Rigidbody2D>());
    }

    public override void MyUpdate()
    {

        float horizontal = input.GetHorizontal();
        float vertical = input.GetVertical();

        UpdateState();

        UpdateLookDirection();

        UpdateAimDirection();

        UpdateAnimations();
        
        mover.Move(new Vector2(horizontal, 0));

        if (Input.GetKeyDown("u"))
        {
            shooter.AssignBullet(0);
        }
        if (Input.GetKeyDown("i"))
        {
            shooter.AssignBullet(1);
        }
    }

    private void UpdateState()
    {
        float horizontal = input.GetHorizontal();
        float vertical = input.GetVertical();
        bool shoot = input.Shoot();
        bool jump = input.Jump();


        switch (_state)
        {
            case State.Idle:
                if (Mathf.Abs(horizontal) > 0.05f) _state = State.Moving;
                if (!grounder.IsGrounded()) _state = State.Jumping;
                if (jump) jumper.Jump();
                if (_isShooting)
                {
                    if(bodyAnimator.IsAnimationFinished())
                    {
                        _isShooting = false;
                    }
                }
                break;
            case State.Moving:
                if (Mathf.Abs(horizontal) <= 0.05f) _state = State.Idle;
                if (!grounder.IsGrounded()) _state = State.Jumping;
                if (jump) jumper.Jump();
                if (_isShooting)
                {
                    if (bodyAnimator.IsAnimationFinished())
                    {
                        _isShooting = false;
                    }
                }
                break;
            case State.Jumping:
                if (grounder.IsGrounded()) _state = State.Idle;
                break;
        }
        if (shoot)
        {
            _isShooting = true;
            _lastShootingTime = Time.time;

            shooter.Shoot(_aimDirection);
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
        }

        if(_state == State.Jumping)
        {
            bodyAnimation = "Jump";
            legsAnimation = "Jump";

            transform.localScale = new Vector3(_lookDirection == LookDirection.Right ? 1 : -1, 1, 1);
        }

        bodyAnimator.Play(bodyAnimation, ignoreBodyAnimation);
        legsAnimator.Play(legsAnimation, ignoreLegsAnimation);
    }
}
