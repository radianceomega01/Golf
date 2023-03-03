using Blueberry.Core.Data;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class JoinRandomRoom : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private Image loadingBar;
    [SerializeField] private TextMeshProUGUI errorMsg;

    private byte maxPlayers = 2;

    private void Update()
    {
        if (loadingPanel.activeInHierarchy)
            loadingBar.fillAmount += 0.01f;
    }

    public void Join()
    {
        loadingPanel.SetActive(true);
        PhotonNetwork.JoinRandomRoom();
    }
    public override void OnJoinedRoom()
    {
        loadingPanel.SetActive(false);
        loadingBar.fillAmount = 0;
        PhotonNetwork.NickName = Session.User.Name.full;
        PhotonNetwork.LoadLevel("RandomRoom");
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        base.OnJoinRandomFailed(returnCode, message);
        string roomName = "Room " + Random.Range(0, 10000);
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;
        roomOptions.MaxPlayers = 2;

        PhotonNetwork.CreateRoom(roomName, roomOptions, ConnectToServer.instance.GetTypedLobby(), null);
    }

    public async override void OnCreateRoomFailed(short returnCode, string message)
    {
        loadingPanel.SetActive(false);
        loadingBar.fillAmount = 0;
        errorMsg.text = message;
        errorMsg.gameObject.SetActive(true);
        await UniTask.Delay(2500);
        errorMsg.gameObject.SetActive(false);
    }

}
