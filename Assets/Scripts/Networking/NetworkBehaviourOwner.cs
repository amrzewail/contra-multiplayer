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

        if (isServer)
        {
            ServerAwake();
        }
        else
        {
            ClientAwake();
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
        if (isServer)
        {
            ServerStart();
        }
        else
        {
            ClientStart();
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
        if (isServer)
        {
            ServerUpdate();
        }
        else
        {
            ClientUpdate();
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
        if (isServer)
        {
            ServerFixedUpdate();
        }
        else
        {
            ClientFixedUpdate();
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
        if (isServer)
        {
            ServerOnDestroy();
        }
        else
        {
            ClientOnDestroy();
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
        if (isServer)
        {
            ServerOnTriggerEnter2D(collider);
        }
        else
        {
            ClientOnTriggerEnter2D(collider);
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
        if (isServer)
        {
            ServerOnTriggerExit2D(collider);
        }
        else
        {
            ClientOnTriggerExit2D(collider);
        }
    }

    #region My

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


    public virtual void MyOnDestroy()
    {

    }


    public virtual void MyOnTriggerEnter2D(Collider2D collider)
    {

    }


    public virtual void MyOnTriggerExit2D(Collider2D collider)
    {

    }

    #endregion My


    #region Other

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

    public virtual void OtherOnDestroy()
    {

    }

    public virtual void OtherOnTriggerEnter2D(Collider2D collider)
    {

    }



    public virtual void OtherOnTriggerExit2D(Collider2D collider)
    {

    }

    #endregion Other

    #region Server
    public virtual void ServerAwake()
    {

    }
    public virtual void ServerStart()
    {

    }
    public virtual void ServerUpdate()
    {

    }
    public virtual void ServerFixedUpdate()
    {

    }

    public virtual void ServerOnDestroy()
    {

    }

    public virtual void ServerOnTriggerEnter2D(Collider2D collider)
    {

    }



    public virtual void ServerOnTriggerExit2D(Collider2D collider)
    {

    }

    #endregion Server

    #region Client
    public virtual void ClientAwake()
    {

    }
    public virtual void ClientStart()
    {

    }
    public virtual void ClientUpdate()
    {

    }
    public virtual void ClientFixedUpdate()
    {

    }

    public virtual void ClientOnDestroy()
    {

    }

    public virtual void ClientOnTriggerEnter2D(Collider2D collider)
    {

    }



    public virtual void ClientOnTriggerExit2D(Collider2D collider)
    {

    }

    #endregion Client
}
