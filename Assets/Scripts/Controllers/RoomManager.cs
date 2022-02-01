using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [SerializeField] string playerManagerName;
    [SerializeField] Transform playerRespawnLocation;

    internal void Start()
    {
        PhotonNetwork.Instantiate($"PhotonPrefabs/{playerManagerName}", playerRespawnLocation.position, Quaternion.identity);
    }
}
