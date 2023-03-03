using Blueberry.Core.SceneManagement;
using Photon.Pun;
using System;
using UnityEngine;
using UnityEngine.UI;

public class BackButton : MonoBehaviour
{
    [SerializeField] private BackOptions loadingScene;
    public static event Action OnPressed;
    Button backButton;
    void Awake()
    {
        backButton = gameObject.transform.GetComponent<Button>();
        backButton.onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        if (loadingScene == BackOptions.CharacterSelectionScreen)
            SceneHandler.Load("Character Selection");
        else if (loadingScene == BackOptions.TitleScreen)
            SceneHandler.Load("Title");
        else if (loadingScene == BackOptions.GameModeScreen)
        {
            if (GameManager.Instance.GameMode != GameManager.GameModes.SinglePlayer)
            {
                OnPressed?.Invoke();
                PhotonNetwork.Disconnect();
            }
            SceneHandler.Load("GameMode");
        }
        else if (loadingScene == BackOptions.LobbyScreen)
        {
            PhotonNetwork.LeaveRoom();
            SceneHandler.Load("Lobby");
        }
        GameManager.Instance.ResetActiveCourse();
    }
}

enum BackOptions
{ 
    CharacterSelectionScreen,
    TitleScreen,
    GameModeScreen,
    LobbyScreen
}
