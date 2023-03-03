using Blueberry.Core.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayButton : MonoBehaviour
{
   private Button playButton;

    private void Awake()
    {
        playButton = GetComponent<Button>();
        playButton.onClick.AddListener(OnButtonClicked);
    }

    public void OnButtonClicked()
    {
        //SceneHandler.Load("Character Selection");
        SceneHandler.Load("GameMode");
    }
}
