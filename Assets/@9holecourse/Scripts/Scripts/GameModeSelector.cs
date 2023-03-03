using Blueberry.Core.SceneManagement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameModeSelector : MonoBehaviour
{
    [SerializeField] private Button singlePlayer;
    [SerializeField] private Button privateRoom;
    [SerializeField] private Button multiPlayer;
    [SerializeField] private Button upgradeSkills;
    [SerializeField] private JoinRandomRoom joinRandomRoom;
    [SerializeField] private GameObject loadingPanel;
    //[SerializeField] private TMP_Dropdown dropDown;
    [SerializeField] private Image loadingBar;

    private string region;
    private ConnectToServer connectToServer;
    void Awake()
    {
        singlePlayer.onClick.AddListener(SetSinglePlayer);
        privateRoom.onClick.AddListener(SetPrivateRoom);
        multiPlayer.onClick.AddListener(SetMultiPlayer);
        upgradeSkills.onClick.AddListener(LoadUpgradeScene);
        //dropDown.onValueChanged.AddListener(GetRegion);
        connectToServer = ConnectToServer.instance;
    }
    private void OnEnable()
    {
        connectToServer.OnComplete += LoadNextScene;
    }

    private void OnDisable()
    {
        connectToServer.OnComplete -= LoadNextScene;
    }
    private void Start()
    {
        connectToServer.Disconnect();
        //GetRegion(0);
    }

    private void Update()
    {
        if (loadingPanel.activeInHierarchy)
        {
            if (!connectToServer.Connected)
                loadingBar.fillAmount += 0.01f;
            else
            {
                loadingPanel.SetActive(false);
                loadingBar.fillAmount = 0;
            }
        }
    }

    private void SetSinglePlayer()
    {
        GameManager.Instance.GameMode = GameManager.GameModes.SinglePlayer;
        Proceed();
    }
    private void SetPrivateRoom()
    {
        GameManager.Instance.GameMode = GameManager.GameModes.CustomRoom;
        Proceed();
    }
    private void SetMultiPlayer()
    {
        GameManager.Instance.GameMode = GameManager.GameModes.MultiPlayer;
        Proceed();
    }

    private void LoadUpgradeScene()
    {
        loadingPanel.SetActive(true);
        SceneHandler.Load("UpgradeSkills");
    }

    private void Proceed()
    {
        loadingPanel.SetActive(true);

        switch (GameManager.Instance.GameMode)
        {
            
            case GameManager.GameModes.SinglePlayer:
                SceneHandler.Load("Character Selection");
                break;

            case GameManager.GameModes.CustomRoom:
                connectToServer.Connect();
                break;

            case GameManager.GameModes.MultiPlayer:
                connectToServer.Connect();

                break;
        }
    }

    private void LoadNextScene(bool value)
    {
        switch (GameManager.Instance.GameMode)
        {
            case GameManager.GameModes.CustomRoom:
                SceneHandler.Load("Lobby");
                break;

            case GameManager.GameModes.MultiPlayer:
                joinRandomRoom.Join();
                break;
        }
    }

    /*private void GetRegion(int value)
    {
        switch (value)
        {
            case 0: region = "us";
                break;
            case 1:
                region = "usw";
                break;
            case 2:
                region = "in";
                break;
            case 3:
                region = "asia";
                break;
            case 4:
                region = "cn";
                break;
            case 5:
                region = "jp";
                break;
            case 6:
                region = "ru";
                break;
        }
        ConnectToServer.instance.SetRegion(region);
    }*/

}
