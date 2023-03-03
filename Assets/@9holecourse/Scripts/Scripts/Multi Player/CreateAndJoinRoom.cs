using Blueberry.Core.Data;
using Blueberry.Modules.OnScreenKeyboard;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateAndJoinRoom : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_InputField input;
    [SerializeField] private Button createRoom;
    [SerializeField] private Button joinRoom;
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private Image loadingBar;
    [SerializeField] private TextMeshProUGUI errorMsg;

    private void Awake()
    {
        createRoom.onClick.AddListener(CreateRoom);
        joinRoom.onClick.AddListener(OnJoinClicked);
        ConnectToServer.instance.OnPlayerDisconnected += ReconnectToServer;
        ConnectToServer.instance.OnComplete += JoinRoom;
        BackButton.OnPressed += DeReference;
    }

    private void OnDestroy() => DeReference();

    private void DeReference()
    {
        ConnectToServer.instance.OnComplete -= JoinRoom;
        ConnectToServer.instance.OnPlayerDisconnected -= ReconnectToServer;
    }
    private void Update()
    {
        if (loadingPanel.activeInHierarchy)
            loadingBar.fillAmount += 0.03f;
    }

    private void CreateRoom()
    {
        int randomNum = UnityEngine.Random.Range(1111, 9999);
        string randomRoom = PhotonNetwork.CloudRegion + randomNum;
        loadingPanel.SetActive(true);
        PhotonNetwork.CreateRoom(randomRoom, new RoomOptions() { MaxPlayers = 4 }, null);
    }

    public async override void OnCreateRoomFailed(short returnCode, string message)
    {
        //base.OnCreateRoomFailed(returnCode, message);
        loadingPanel.SetActive(false);
        loadingBar.fillAmount = 0;
        errorMsg.text = message;
        errorMsg.gameObject.SetActive(true);
        await UniTask.Delay(2500);
        errorMsg.gameObject.SetActive(false);
        
    }

    private void OnJoinClicked()
    {
        if (input.text.Length < 6)
            return;
        loadingPanel.SetActive(true);
        ConnectToServer.instance.Disconnect();
    }

    private void ReconnectToServer(DisconnectCause cause)
    {
        var region = new string(input.text.Where(char.IsLetter).ToArray());
        Debug.Log("joinRegion: " + region);
        ConnectToServer.instance.ConnectWithRegion(region);
    }
    private void JoinRoom(bool status) => PhotonNetwork.JoinRoom(input.text);
    public async override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
        loadingPanel.SetActive(false);
        loadingBar.fillAmount = 0;
        errorMsg.text = "Join Failed! Check Connection or Room Name";
        errorMsg.gameObject.SetActive(true);
        await UniTask.Delay(2500);
        errorMsg.gameObject.SetActive(false);
    }

    public override void OnJoinedRoom()
    {
        loadingPanel.SetActive(false);
        loadingBar.fillAmount = 0;
        PhotonNetwork.NickName = Session.User.Name.full;
        PhotonNetwork.LoadLevel("CustomRoom");
    }

}
