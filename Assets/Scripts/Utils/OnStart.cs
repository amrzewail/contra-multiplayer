using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnStart : MonoBehaviourOwner
{
    public bool serverAuthority;

    public UnityEvent StartEvent;

    public override void MyStart()
    {
        if (serverAuthority) return;
        StartEvent?.Invoke();
    }

    public override void ServerStart()
    {
        if (!serverAuthority) return;
        StartEvent?.Invoke();
    }

}
