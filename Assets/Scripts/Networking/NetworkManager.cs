using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    internal void Awake()
    {
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    // Update is called once per frame
    internal void Update()
    {
        if (Input.GetKeyDown("f"))
        {
            if(PhotonNetwork.IsMasterClient) PhotonNetwork.LoadLevel(1);
        }
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnected();

        Debug.Log("Photon:: Connected");

        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();

        Debug.Log("Photon:: Joined Lobby");
        PhotonNetwork.JoinRandomOrCreateRoom();

    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        Debug.Log($"Photon:: Joined Room {PhotonNetwork.CurrentRoom.Name}");
    }
}
