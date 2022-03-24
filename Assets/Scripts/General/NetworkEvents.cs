using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkEvents : MonoBehaviour
{
    public static Action OnClientConnect;
    public static Action OnClientDisconnect; //Called on clients when disconnected from the server

    public static Action OnHostConnect;
    public static Action OnHostDisconnect;

    private void Awake()
    {
        ResetEvents();
    }

    private void OnDestroy()
    {
        ResetEvents();
    }

    private void ResetEvents()
    {
        OnClientConnect = null;
        OnClientDisconnect = null;
        OnHostConnect = null;
        OnHostDisconnect = null;
    }
}
