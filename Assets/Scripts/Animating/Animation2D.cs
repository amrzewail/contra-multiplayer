using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.sigma.scripts.data.animation
{

    [CreateAssetMenu(fileName = "Animation2D", menuName = "Sigma/Animation/Animation2D")]
    public class Animation2D : ScriptableObject, IAnimation
    {
        public string identifier;
        public bool loop = false;
        public float speed = 10;

        public List<Sprite> frames;
        public Action onComplete;

        [SerializeField] List<AnimationEvent> animationEvents;

        public float interval
        {
            get { return 1f / speed; }
        }

        public float duration
        {
            get { return interval * frames.Count; }
        }

        public new string name => identifier;

        public int length => frames.Count;

        bool IAnimation.loop => this.loop;

        public Sprite GetFrame(int index)
        {
            if (index >= length)
                if (loop) index = index % length;
                else index = length - 1;
            return frames[index];
        }

        public void invokeCompletion()
        {
            onComplete?.Invoke();
            onComplete = null;
        }


        public void UpdateEvents(int currentFrame, ref List<string> invokeList)
        {
            foreach (var ev in animationEvents)
            {
                if (ev.frame == currentFrame) invokeList.Add(ev.eventName);
            }
        }

        [System.Serializable]
        public class AnimationEvent
        {
            public int frame;
            public string eventName;
        }
    }
}