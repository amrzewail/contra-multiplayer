using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAnimator
{
    Action<IAnimation> AnimationChanged { get; set; }

    Action<string> AnimationEvent { get; set; }

    IAnimatorController controller { get; set; }

    IAnimation GetCurrentAnimation();

    void Play(string animationName, bool ignoreCurrentAnimation = false);

    bool IsPlaying(string animationName);

    bool IsAnimationFinished();

    float GetNormalizedTime();

    void SetNormalizedTime(float time);
}
