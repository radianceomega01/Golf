
using Photon.Pun;
using Photon.Realtime;
using System;
using UnityEngine;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    private ServerSettings serverSettings;
    public bool Connected { get; private set; }

    public event Action<bool> OnComplete;
    public event Action<Player> OnPlayerLeft;
    public event Action<Player> OnPlayerEntered;
    public event Action OnPlayerConnected;
    public event Action<DisconnectCause> OnPlayerDisconnected;

    private TypedLobby golfLobby = new TypedLobby("9Hole", LobbyType.Default);
    public static ConnectToServer instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        serverSettings = PhotonNetwork.PhotonServerSettings;
        serverSettings.DevRegion = null;
        ServerSettings.ResetBestRegionCodeInPreferences();
    }

    public void Connect()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public void ConnectWithRegion(string region)
    {
        serverSettings.AppSettings.FixedRegion = region;
        PhotonNetwork.ConnectUsingSettings(serverSettings.AppSettings); 
    }

    public void Disconnect()
    {
        Connected = false;
        PhotonNetwork.Disconnect();
    }

    public override void OnConnectedToMaster() => PhotonNetwork.JoinLobby(golfLobby);

    public override void OnJoinedLobby()
    {
        Connected = true;
        OnComplete?.Invoke(true);
    }

    public TypedLobby GetTypedLobby() => golfLobby;
    public override void OnPlayerEnteredRoom(Player newPlayer) => OnPlayerEntered?.Invoke(newPlayer);
    public override void OnPlayerLeftRoom(Player otherPlayer) => OnPlayerLeft?.Invoke(otherPlayer);

    public override void OnConnected() => OnPlayerConnected?.Invoke();
    public override void OnDisconnected(DisconnectCause cause) => OnPlayerDisconnected?.Invoke(cause);
}
