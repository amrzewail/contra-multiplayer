using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.sigma.scripts.data.animation
{

    [CreateAssetMenu(fileName = "Controller2D", menuName = "Sigma/Animation/AnimatorController2D")]
    public class AnimatorController2D : ScriptableObject, IAnimatorController
    {
        [SerializeField] [RequireInterface(typeof(IAnimation))] Object[] _animations;

        public IAnimation GetAnimation(int index)
        {
            if (index >= _animations.Length) return null;
            return (IAnimation)(_animations[index]);
        }

        public IAnimation GetAnimation(string name)
        {
            IAnimation animation = null;
            foreach (var anim in _animations)
            {
                animation = (IAnimation)anim;
                if (animation.name.Equals(name)) return animation;
            }
            return null;
        }

        public bool HasAnimations()
        {
            return _animations != null && _animations.Length > 0; 
        }
    }
}