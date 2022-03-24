using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [System.Serializable]
    public class Sound
    {
        public SFX type;
        public AudioClip clip;
    }

    [SerializeField] AudioSource source;
    [SerializeField] Sound[] sounds;

    internal void Start()
    {
        SoundEvents.Play += PlayCallback;
        SoundEvents.StopBackground += StopBackgroundCallback;
    }

    internal void OnDestroy()
    {
        SoundEvents.Play -= PlayCallback;
        SoundEvents.StopBackground -= StopBackgroundCallback;
    }

    private void StopBackgroundCallback()
    {
        Object.FindObjectsOfType<AudioSource>().ToList().Single(x => x.name.Equals("Background")).Stop();
    }

    private void PlayCallback(SFX type)
    {
        foreach(var s in sounds)
        {
            if(s.type == type)
            {
                source.PlayOneShot(s.clip);
                break;
            }
        }
    }
}
