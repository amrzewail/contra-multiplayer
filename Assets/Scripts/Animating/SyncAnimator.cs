using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Object = UnityEngine.Object;

public class SyncAnimator : MonoBehaviour, IAnimator
{
    [SerializeField] SpriteRenderer _spriteRenderer;

    [SerializeField] [RequireInterface(typeof(IAnimator))] Object _parentAnimator;
    [SerializeField] [RequireInterface(typeof(IAnimatorController))] Object _controller;

    private IAnimation _currentAnimation;
    private int _currentFrame = 0;
    private float _time;

    public Action<IAnimation> AnimationChanged { get; set; }

    public Action<string> AnimationEvent { get; set; }
    public IAnimatorController controller
    {
        get => (IAnimatorController)_controller; set => _controller = (Object)value;
    }

    public IAnimator parentAnimator => (IAnimator)_parentAnimator;

    public bool isPlaying
    {
        get; private set;
    }

    public int currentFrameIndex
    {
        get { return _currentFrame; }
    }

    public IAnimation currentAnimation
    {
        get { return _currentAnimation; }
    }
    private void Reset()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        if(parentAnimator != null)
        {
            parentAnimator.AnimationChanged += UpdateAnimation;
            UpdateAnimation(parentAnimator.GetCurrentAnimation());
        }
    }

    private void Update()
    {
        if (controller.HasAnimations())
        {
            if (_currentAnimation == null) return;

            float normalizedTime = parentAnimator.GetNormalizedTime();
            SetNormalizedTime(normalizedTime);
            int frameIndex = (int)((normalizedTime/* + (1f / _currentAnimation.length)*/) * _currentAnimation.length) - 1;
            if (frameIndex < 0) frameIndex = 0;
            _spriteRenderer.sprite = _currentAnimation.GetFrame(frameIndex);
        }
    }

    private void UpdateAnimation(IAnimation animation)
    {
        IAnimation anim = null;
        if(animation != null)
        {
            anim = controller.GetAnimation(animation.name);
        }
        if (anim != null)
        {
            _currentAnimation = anim;
            AnimationChanged?.Invoke(anim);
        }
        else
        {
            _currentAnimation = null;
            _spriteRenderer.sprite = null;
        }
    }

    public IAnimation GetCurrentAnimation()
    {
        return _currentAnimation;
    }
    public float GetNormalizedTime()
    {
        if (_currentAnimation == null) return 0;
        return _time / (currentAnimation.interval * currentAnimation.length);
    }

    public bool IsAnimationFinished()
    {
        if (_currentFrame >= currentAnimation.length)
            return true;
        else
            return false;
    }
    public bool IsPlaying(string animationName)
    {
        if (_currentAnimation == null) return false;
        return _currentAnimation.name.Equals(animationName);
    }

    public void Play(string animationName, bool ignoreCurrentAnimation = false)
    {
        if (!IsPlaying(animationName) || ignoreCurrentAnimation)
        {
            _currentAnimation = controller.GetAnimation(animationName);
            _currentFrame = 0;
            _time = currentAnimation.interval * _currentFrame;
            isPlaying = true;
            AnimationChanged?.Invoke(_currentAnimation);
        }
    }

    public void SetNormalizedTime(float time)
    {
        _time = currentAnimation.interval * (((float)currentAnimation.length) * time);
    }
}
