using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Explosion : MonoBehaviour
{
    private IAnimator _animator;

    public UnityEvent OnExplosionEnded;

    private void Start()
    {
        _animator = GetComponentInChildren<IAnimator>();
    }

    private void Update()
    {
        if (_animator.IsAnimationFinished())
        {
            Destroy(this.gameObject);
            OnExplosionEnded?.Invoke();
        }
    }
}
