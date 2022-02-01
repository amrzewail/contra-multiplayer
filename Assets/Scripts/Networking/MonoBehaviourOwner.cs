using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public abstract class MonoBehaviourOwner : MonoBehaviourPunCallbacks
{
    protected PhotonView _photonView;

    public virtual void Awake()
    {
        Transform trans = transform;
        while (!_photonView)
        {
            _photonView = trans.GetComponent<PhotonView>();
            if (!trans.parent) break;
            trans = trans.parent;
        }

        if (_photonView.IsMine)
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
        if (_photonView.IsMine)
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
        if (_photonView.IsMine)
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
        if (_photonView.IsMine)
        {
            MyFixedUpdate();
        }
        else
        {
            OtherFixedUpdate();
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
}
