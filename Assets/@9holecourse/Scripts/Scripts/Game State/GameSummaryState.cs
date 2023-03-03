using Blueberry.Core.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSummaryState : SingletonBehaviour<GameSummaryState>, IGameState
{
    [SerializeField] private Canvas parentView;
    [SerializeField] private Button nextBtn;

    protected override void Awake()
    {
        base.Awake();

        GameStateManager.Instance.GameStates.Add(this.name, this);
        nextBtn.onClick.AddListener(BackToMainScreen);

        Exit();
    }
    public void Enter()
    {
        parentView.gameObject.SetActive(true);
        ScoreboardUsersUI.Instance.Initialize();
        ScoreboardScoresUI.Instance.Initialize();
    }

    public void Exit()
    {
        parentView.gameObject.SetActive(false);
    }

    public void Process() { }
    private void BackToMainScreen()
    {
        //GameManager.Instance.Proceed();
        SceneHandler.Load("GameMode");
    }
}
