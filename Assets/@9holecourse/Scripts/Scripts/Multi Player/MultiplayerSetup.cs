
using UnityEngine;
using Photon.Realtime;
using Blueberry.Core.Data;
using Photon.Pun;
using System.Collections.Generic;

public class MultiplayerSetup : MonoBehaviour
{
    [SerializeField] private List<string> newRPCList;

    void Start()
    {
        PhotonNetwork.PhotonServerSettings.RpcList = newRPCList;
    }
}
