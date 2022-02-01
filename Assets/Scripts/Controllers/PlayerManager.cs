using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    private PhotonView _photonView;
    private GameObject _instantiatedPlayer;

    [SerializeField] string playerName;

    // Start is called before the first frame update
    void Start()
    {
        _photonView = GetComponentInChildren<PhotonView>();

        if (_photonView.IsMine) Spawn();
    }

    public void Spawn()
    {
        _instantiatedPlayer = PhotonNetwork.Instantiate($"PhotonPrefabs/{playerName}", transform.position, transform.rotation);
    }

    public void Destroy()
    {
        PhotonNetwork.Destroy(_instantiatedPlayer);
    }
}
