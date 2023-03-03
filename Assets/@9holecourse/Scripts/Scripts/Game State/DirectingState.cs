using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DirectingState : SingletonBehaviour<DirectingState>, IGameState
{
    [Header("View")]
    [SerializeField] private GameObject staticView;
    [SerializeField] private GameObject dynamicView;
    [SerializeField] private GameObject inputPanel;
    [SerializeField] private Button shoot;
    [SerializeField] private Button resetCamera;
    [SerializeField] private PositionIndicator holeIndicator;
    [SerializeField] private PositionIndicator golferIndicator;
    [SerializeField] private Transform opponentIndicatorHolder;
    [SerializeField] private GameObject opponentIndicator;

    //private Canvas canvas;
    private ShotAngleController angleController;
    private new OverviewCamera camera;
    private Golfer golfer;
    private Transform hole;

    protected override void Awake()
    {
        base.Awake();

        GameStateManager.Instance.GameStates.Add(this.name, this);

        //canvas = GetComponentInChildren<Canvas>();
        angleController = FindObjectOfType<ShotAngleController>(true);
        camera = FindObjectOfType<OverviewCamera>(true);
        hole = CourseManager.Instance.ActiveCourse.hole;

        camera.OnLive += OnCameraLive;
        shoot.onClick.AddListener(OnShootClicked);
        resetCamera.onClick.AddListener(OnResetCameraClicked);

        holeIndicator.Target = CourseManager.Instance.ActiveCourse.hole;

        Exit();
    }

    public void Enter()
    {
        Trajectory.Instance.SetWidthMultiplier(1);

        if (GameManager.Instance.GameMode == GameManager.GameModes.SinglePlayer)
        {
            WindManager.Instance.RandomizeWind();
        }
        else
        {
            if (MatchManager.Instance.GetActiveGolfer().PhotonView.IsMine)
                WindManager.Instance.RandomizeWind();
        }
        
        staticView.SetActive(true);
        dynamicView.SetActive(true);
        camera.gameObject.SetActive(true);
        golfer = MatchManager.Instance.GetActiveGolfer();
        golfer.transform.LookAt(hole);
        SetPositionIndicators();
        ToggleSelfIndicator();
        camera.SetTargetPosition(golfer.transform.position);
        camera.SetTargetRotaion(golfer.transform.rotation);
    }

    public void Exit()
    {
        //canvas.gameObject.SetActive(false);
        staticView.SetActive(false);
        dynamicView.SetActive(false);
        camera.gameObject.SetActive(false);
        angleController.gameObject.SetActive(false);
    }

    public void Process() 
    {
        if (MatchManager.Instance.IsUserGolfer)
            inputPanel.SetActive(true);
        else
            inputPanel.SetActive(false);
    }

    private void OnCameraLive() => angleController.gameObject.SetActive(true);

    private void OnShootClicked()
    {
        if (angleController.Valid)
        {
            Trajectory.Instance.Clear();
            GameStateManager.Instance.SetState(ShootingState.Instance.name);
        }
    }

    private void SetPositionIndicators()
    {
        GameObject instantiatedObject;

        for (int i = 0; i < opponentIndicatorHolder.childCount; i++)
            Destroy(opponentIndicatorHolder.GetChild(i).gameObject);

        foreach (Golfer golfer in MatchManager.Instance.GetAllGolfers())
        {
            if (golfer == null)
                continue;

            if (golfer.Scorecard.Id == MatchManager.Instance.UserId)
            {
                Transform golferBall = golfer.Ball.transform;
                golferIndicator.Target = golferBall;
                golferIndicator.Name = golfer.Scorecard.name;
                if (golfer.Scorecard.CourseComplete)
                    golferIndicator.gameObject.SetActive(false);
            }

            else if (GameManager.Instance.GameMode == GameManager.GameModes.SinglePlayer)
            {
                Transform opponentBall = MatchManager.Instance.GetGolferWithId(MatchManager.Instance.UserId + 1).Ball.transform;
                instantiatedObject = Instantiate(opponentIndicator, opponentIndicatorHolder);
                instantiatedObject.GetComponent<PositionIndicator>().Target = opponentBall;
                instantiatedObject.GetComponent<PositionIndicator>().Name = MatchManager.Instance.GetGolferWithId(MatchManager.Instance.UserId + 1).GetCharacter().name;
                ToggleOpponentIndicator(golfer, instantiatedObject);
            }
            else
            {
                Transform opponentBall = golfer.Ball.transform;
                instantiatedObject = Instantiate(opponentIndicator, opponentIndicatorHolder);
                instantiatedObject.GetComponent<PositionIndicator>().Target = opponentBall;
                instantiatedObject.GetComponent<PositionIndicator>().Name = golfer.Scorecard.name;
                ToggleOpponentIndicator(golfer, instantiatedObject);
            }
        }
    }

    private void ToggleSelfIndicator()
    {
        if (MatchManager.Instance.GetGolferWithId(MatchManager.Instance.UserId).Scorecard.CourseComplete)
            golferIndicator.gameObject.SetActive(false);
    }

    private void ToggleOpponentIndicator(Golfer golfer, GameObject indicator)
    {
        if (golfer.Scorecard.CourseComplete)
        {
            indicator.SetActive(false);
            return;
        }

        if (golfer.Ball.transform.position == MatchManager.Instance.GetGolferWithId(MatchManager.Instance.UserId).Ball.transform.position)
        {
            if (golfer == MatchManager.Instance.GetActiveGolfer())
            {
                indicator.SetActive(true);
                golferIndicator.gameObject.SetActive(false);
            }

            else
            {
                indicator.SetActive(false);
                golferIndicator.gameObject.SetActive(true);
            }
        }
        else
        {
            indicator.SetActive(true);
            golferIndicator.gameObject.SetActive(true);
        }

    }

    private void OnResetCameraClicked() => camera.SetTargetPosition(golfer.transform.position);
}
