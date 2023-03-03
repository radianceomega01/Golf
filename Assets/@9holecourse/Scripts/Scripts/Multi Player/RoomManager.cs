using Blueberry.Core.AddressableAssets;
using Blueberry.Core.Data;
using Blueberry.Core.SceneManagement;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class RoomManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    [SerializeField] private TextMeshProUGUI roomName;
    [SerializeField] private Transform playerHolder;
    [SerializeField] private TextMeshProUGUI playerCount;
    [SerializeField] private Button nextButton;

    GameObject roomPlayerPrefab;
    RoomPlayer roomPlayer;
    RoomPlayer roomSelfPlayer;
    CustomInstantiator customInstantiator;
    List<RoomPlayer> playerList = new List<RoomPlayer>();
    Queue playersReady = new Queue();
    const int customReadyCode = 1;

    private void Awake()
    {
        ConnectToServer.instance.OnPlayerEntered += SetPlayerCount;
        ConnectToServer.instance.OnPlayerLeft += SetPlayerCount;
        ConnectToServer.instance.OnPlayerLeft += RemovePlayer;
        nextButton.onClick.AddListener(SetPlayerReady);
    }


    
    private void Start()
    {
        customInstantiator = new CustomInstantiator();
        customInstantiator.OnEventRaised += AddOtherPlayers;

        if (PhotonNetwork.CurrentRoom != null)
            roomName.text = PhotonNetwork.CurrentRoom.Name;

        GetRoomPlayers();
        CreatePlayer();
        SetPlayerCount(null);

    }

    private void GetRoomPlayers()
    {
        AddOtherPlayers(CustomInstantiator.playerCache);
    }

    private void CreatePlayer()
    {
        if (PhotonNetwork.CurrentRoom == null)
            return;

        foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
        {
            if (player.Value.NickName == Session.User.Name.full)
            {
                AddSelfPlayer(player.Value);
                break;
            }
        }
    }
    
    private void SetPlayerReady()
    {
        if (PhotonNetwork.PlayerList.Length > 1)
        {
            roomSelfPlayer.GetPhotonView().RPC("SetReady", RpcTarget.All, roomSelfPlayer.GetPhotonView().ViewID);
            photonView.RPC("AllPlayersReady", RpcTarget.All);
            //nextButton.interactable = false;
            nextButton.gameObject.SetActive(false);

            //Raising an event for creating a cache of ready players for upcomming players to join
            object[] data = new object[]
            {
            roomSelfPlayer.GetPhotonView().ViewID
            };

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions
            {
                Receivers = ReceiverGroup.Others,
                CachingOption = EventCaching.AddToRoomCache
            };

            SendOptions sendOptions = new SendOptions
            {
                Reliability = true
            };
            PhotonNetwork.RaiseEvent(customReadyCode, data, raiseEventOptions, sendOptions);
        }
    }

    [PunRPC]
    private void AllPlayersReady()
    {
        foreach (RoomPlayer roomPlayer in playerList)
        {
            if (!roomPlayer.IsReady)
                return;
        }
        //SceneHandler.Load("Character Selection");

        PhotonNetwork.LoadLevel("Character Selection");
    }

    private async void AddOtherPlayers(Queue playerQueue)
    {
        GameObject playerObj;
        if(roomPlayerPrefab == null)
            roomPlayerPrefab = await AddressablesManager.LoadAsset<GameObject>("RoomPlayer");

        while (playerQueue.Count > 0)
        {
            EventData playerData = (EventData)playerQueue.Dequeue();
            object[] data = (object[])playerData.CustomData;
            object[] specificData = (object[])data[1];

            playerObj = Instantiate(roomPlayerPrefab, playerHolder);
            PhotonView photonView = playerObj.GetComponent<PhotonView>();
            photonView.ViewID = (int)data[0];
            RoomPlayer roomPlayer = playerObj.GetComponent<RoomPlayer>();
            if(specificData[0] == null || specificData[0].Equals(""))
                roomPlayer.SetData(data[0].ToString(), (int)specificData[1]);
            else
                roomPlayer.SetData(specificData[0].ToString(), (int)specificData[1]);

            //making player ready who joined and pressed ready before hand
            if (playersReady.Contains(data[0]))
                roomPlayer.SetReady((int)data[0]);

            playerList.Add(roomPlayer);
        }
    }
    private async void AddSelfPlayer(Player player)
    {
        if (roomPlayerPrefab == null)
            roomPlayerPrefab = await AddressablesManager.LoadAsset<GameObject>("RoomPlayer");

        object[] specificData = new object[] { player.NickName, player.ActorNumber };
        GameObject roomPlayerObject = customInstantiator.Instantiate(roomPlayerPrefab, playerHolder, specificData);
        roomPlayer = roomPlayerObject.GetComponent<RoomPlayer>();
        if(player.NickName == null || player.NickName == "")
            roomPlayer.SetData(roomPlayer.GetPhotonView().ViewID.ToString(), player.ActorNumber);
        else
            roomPlayer.SetData(player.NickName, player.ActorNumber);
        playerList.Add(roomPlayer);

        if (player.NickName == Session.User.Name.full)
            roomSelfPlayer = roomPlayer;
    }

    private void RemovePlayer(Player player)
    {
        for (int i = 0; i < playerList.Count; i++)
        {
            if (playerList[i].GetComponent<RoomPlayer>().ActorNum == player.ActorNumber)
            {
                playerList.RemoveAt(i);
                break;
            }
                
        }

        for (int i = 0; i < playerHolder.childCount; i++)
        {
            if (playerHolder.GetChild(i).GetComponent<RoomPlayer>().ActorNum == player.ActorNumber)
            {
                Destroy(playerHolder.GetChild(i).gameObject);
                break;
            }

        }
    }

    private void SetPlayerCount(Player player)
    {
        if (playerCount != null)
        {
            playerCount.text = PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers;
        }
    }
    private void OnDestroy()
    {
        customInstantiator.OnEventRaised -= AddOtherPlayers;
        ConnectToServer.instance.OnPlayerEntered -= SetPlayerCount;
        ConnectToServer.instance.OnPlayerLeft -= SetPlayerCount;
        ConnectToServer.instance.OnPlayerLeft -= RemovePlayer;
    }

    public void OnEvent(EventData photonEvent)
    {
        object[] data;
        if (photonEvent.Code == customReadyCode)
        {
            data = (object[])photonEvent.CustomData;
            playersReady.Enqueue(data[0]);
        }
    }
}
