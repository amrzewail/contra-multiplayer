using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool isInvader = false;

    public static GameManager instance { get; private set; }



    internal void Awake()
    {
        if (instance)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);

        Application.targetFrameRate = 120;
    }

    internal void OnDestroy()
    {
        isInvader = false;
    }
}
