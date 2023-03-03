using Blueberry.Core.SceneManagement;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MatchInitializer : MonoBehaviour
{
    async void Start()
    {
        if (GameManager.Instance.GameMode == GameManager.GameModes.SinglePlayer)
        {
            await SceneHandler.Load("Course", LoadSceneMode.Additive);
            SceneHandler.Load("States", LoadSceneMode.Additive);
        }
        else
        {
            PhotonNetwork.LoadLevel("Course", LoadSceneMode.Additive);
            PhotonNetwork.LoadLevel("States", LoadSceneMode.Additive);
        }

    }
}
