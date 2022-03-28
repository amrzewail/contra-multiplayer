using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool isInvader = false;

    public static GameManager instance { get; private set; }



    internal void Awake()
    {
#if !UNITY_EDITOR
        //Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Locked;
#endif
        if (instance)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);

        Application.targetFrameRate = 120;
    }

    internal void Update()
    {

    }
    internal void OnDestroy()
    {
        isInvader = false;
    }
}
