using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Object = UnityEngine.Object;

public class Animator2D : MonoBehaviour, IAnimator
{

    private IAnimation _currentAnimation;
    private int _currentFrame = 0;
    private float _time;
    private List<string> _invokationList;

    public SpriteRenderer spriteRenderer;

    [SerializeField] [RequireInterface(typeof(IAnimatorController))] Object _controller;
    [SerializeField] bool playFirstAnimation = true;

    public Action<IAnimation> AnimationChanged { get; set; }

    public Action<string> AnimationEvent { get; set; }

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


    public IAnimatorController controller
    {
        get => (IAnimatorController)_controller; set => _controller = (Object)value;
    }

    private void Start()
    {
        _invokationList = new List<string>();
        if(playFirstAnimation) Play(controller.GetAnimation(0).name, true);
    }

    private void Reset()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (isPlaying)
        {
            UpdatePlaying();
        }

    }

    public void UpdatePlaying()
    {
        if (controller.HasAnimations())
        {
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying)
                _time += 1f / 30;
            else
                _time += Time.deltaTime;
#else
                    _time += Time.deltaTime;
#endif
            if (!IsAnimationFinished())
            {
                if (_time > currentAnimation.interval * (_currentFrame + 1))
                {

                    spriteRenderer.sprite = GetCurrentFrame();
                    _currentFrame = NextFrameIndex();

                    _currentAnimation.UpdateEvents(_currentFrame, ref _invokationList);
                    foreach (var inv in _invokationList) AnimationEvent?.Invoke(inv);
                    _invokationList.Clear();
                }
            }
            else
            {
                if (currentAnimation.loop)
                {
                    Play(_currentAnimation.name, true);
                }
                else
                {
                    isPlaying = false;
                    //currentAnimation.invokeCompletion();
                }
            }
        }
    }

    private int NextFrameIndex()
    {
        if (IsAnimationFinished())
            return currentAnimation.length - 1;
        else
            return _currentFrame + 1;
    }


    private Sprite GetCurrentFrame()
    {
        int count = currentAnimation.length;
        if (count > 0)
        {
            if (_currentFrame >= count)
                return currentAnimation.GetFrame(count - 1);
            return currentAnimation.GetFrame(_currentFrame);
        }
        return null;
    }

    private void PlayInternal(string animationName, bool ignoreCurrentAnimation, bool invokeChange)
    {
        if (!IsPlaying(animationName) || ignoreCurrentAnimation)
        {
            _currentAnimation = controller.GetAnimation(animationName);
            _currentFrame = 0;
            _time = currentAnimation.interval * _currentFrame;
            isPlaying = true;
            if (invokeChange)
            {
                AnimationChanged?.Invoke(_currentAnimation);
            }
        }
    }

    public IAnimation GetCurrentAnimation()
    {
        return _currentAnimation;
    }

    public float GetNormalizedTime()
    {
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
        bool invokeChanged = false;
        if ((invokeChanged = !IsPlaying(animationName)) || ignoreCurrentAnimation)
        {
            _currentAnimation = controller.GetAnimation(animationName);
            _currentFrame = 0;
            _time = currentAnimation.interval * _currentFrame;
            isPlaying = true;
            if (invokeChanged)
            {
                AnimationChanged?.Invoke(_currentAnimation);
            }
        }
    }

    public void SetNormalizedTime(float time)
    {
        _time = currentAnimation.interval * (((float)currentAnimation.length) * time);
    }

}
