using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NetworkBehaviourOwner : NetworkBehaviour
{
    protected NetworkIdentity identity => netIdentity;

    public bool isMine => identity.isLocalPlayer || identity.hasAuthority;

    public virtual void Awake()
    {

        if (isMine)
        {
            MyAwake();
        }
        else
        {
            OtherAwake();
        }
    }

    public virtual void Start()
    {
        if (isMine)
        {
            MyStart();
        }
        else
        {
            OtherStart();
        }
    }

    public virtual void Update()
    {
        if (isMine)
        {
            MyUpdate();
        }
        else
        {
            OtherUpdate();
        }

    }

    public virtual void FixedUpdate()
    {
        if (isMine)
        {
            MyFixedUpdate();
        }
        else
        {
            OtherFixedUpdate();
        }
    }
    public virtual void OnDestroy()
    {
        if (isMine)
        {
            MyOnDestroy();
        }
        else
        {
            OtherOnDestroy();
        }
    }


    public virtual void OnTriggerEnter2D(Collider2D collider)
    {
        if (isMine)
        {
            MyOnTriggerEnter2D(collider);
        }
        else
        {
            OtherOnTriggerEnter2D(collider);
        }
    }

    public virtual void OnTriggerExit2D(Collider2D collider)
    {
        if (isMine)
        {
            MyOnTriggerExit2D(collider);
        }
        else
        {
            OtherOnTriggerExit2D(collider);
        }
    }


    public virtual void MyAwake()
    {

    }
    public virtual void MyStart()
    {

    }
    public virtual void MyUpdate()
    {

    }
    public virtual void MyFixedUpdate()
    {

    }
    public virtual void OtherAwake()
    {

    }
    public virtual void OtherStart()
    {

    }
    public virtual void OtherUpdate()
    {

    }
    public virtual void OtherFixedUpdate()
    {

    }
    public virtual void MyOnDestroy()
    {

    }

    public virtual void OtherOnDestroy()
    {

    }

    public virtual void MyOnTriggerEnter2D(Collider2D collider)
    {

    }
    public virtual void OtherOnTriggerEnter2D(Collider2D collider)
    {

    }


    public virtual void MyOnTriggerExit2D(Collider2D collider)
    {

    }

    public virtual void OtherOnTriggerExit2D(Collider2D collider)
    {

    }
}
