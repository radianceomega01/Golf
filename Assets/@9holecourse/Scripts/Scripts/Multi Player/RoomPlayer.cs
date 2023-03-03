using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomPlayer : MonoBehaviour
{
    [SerializeField] private new TextMeshProUGUI name;
    [SerializeField] private Image background;
    private PhotonView photonV;

    public int ActorNum { get; set; }
    public bool IsReady { get; private set; }

    public PhotonView GetPhotonView() => photonV;
    private void Awake()
    {
        photonV = GetComponent<PhotonView>();
    }

    public void SetData(string name, int actorNum)
    {
        this.name.text = name;
        ActorNum = actorNum;
    }

    public string GetName() => name.text;

    [PunRPC]
    public void SetReady(int viewId)
    {
        if (photonV.ViewID == viewId)
        {
            background.color = Color.green;
            IsReady = true;
        }
    }

}
