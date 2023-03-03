using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MatchSummaryState : SingletonBehaviour<MatchSummaryState>, IGameState
{
    [SerializeField] private Canvas parentView;
    [SerializeField] private ScoreboardUI scoreboardUI;
    [SerializeField] private Button nextBtn;
    [SerializeField] private GameObject timerText;
    [SerializeField] private GameObject opponentCardHolder;
    [SerializeField] private Timer timer;

    private Cinematics winCinematics;
    private Cinematics looseCinematics;
    private Golfer golfer;
    private MatchResult matchResult;
    private float timerTime = 10;

    protected override void Awake()
    {
        base.Awake();

        GameStateManager.Instance.GameStates.Add(this.name, this);

        Transform activeCourse = CourseManager.Instance.ActiveCourse.transform;
        winCinematics = activeCourse.Find("Win Cinematics").GetComponent<Cinematics>();
        looseCinematics = activeCourse.Find("Loose Cinematics").GetComponent<Cinematics>();
        nextBtn.onClick.AddListener(ProceedToNextCourse);
        
        Exit();
    }
    public void Enter()
    {
        winCinematics.OnSequenceComplete += OnCinematicSequenceComplete;
        looseCinematics.OnSequenceComplete += OnCinematicSequenceComplete;

        golfer = MatchManager.Instance.GetGolferWithId(MatchManager.Instance.UserId);
        golfer.gameObject.SetActive(true);
        if (golfer.Scorecard.CourseComplete)
            golfer.transform.position = new Vector3(golfer.transform.position.x, CourseManager.Instance.ActiveCourse.hole.position.y, golfer.transform.position.z);
        golfer.ToggleClubHolder(false);
        golfer.ToggleBall(false);
        matchResult = MatchManager.Instance.EvaluateMatchResult();
        if(matchResult == MatchResult.Won)
            golfer.SetAnimationState(Golfer.AnimationState.Win);
        else
            golfer.SetAnimationState(Golfer.AnimationState.Loose);

        winCinematics.transform.position = golfer.transform.position;
        winCinematics.transform.rotation = golfer.transform.rotation;
        looseCinematics.transform.position = golfer.transform.position;
        looseCinematics.transform.rotation = golfer.transform.rotation;

        parentView.gameObject.SetActive(false);
        winCinematics.gameObject.SetActive(matchResult == MatchResult.Won);
        looseCinematics.gameObject.SetActive(matchResult == MatchResult.Lost);
    }

    public void Exit()
    {
        parentView.gameObject.SetActive(false);

        winCinematics.gameObject.SetActive(false);
        looseCinematics.gameObject.SetActive(false);

        winCinematics.OnSequenceComplete -= OnCinematicSequenceComplete;
        looseCinematics.OnSequenceComplete -= OnCinematicSequenceComplete;
        timer.OnTimerComplete -= ProceedToNextCourse;
    }

    public void Process()
    {
        
    }

    private void OnCinematicSequenceComplete()
    {
        scoreboardUI.SetScoreboardData();
        parentView.gameObject.SetActive(true);
        opponentCardHolder.SetActive(false);
        if (GameManager.Instance.GameMode != GameManager.GameModes.SinglePlayer)
        {
            timer.OnTimerComplete += ProceedToNextCourse;
            nextBtn.gameObject.SetActive(false);
            timerText.gameObject.SetActive(true);
            timer.StartTimer(timerTime);
        }
    }

    private void ProceedToNextCourse()
    {
        GameManager.Instance.Proceed();
    }
}
