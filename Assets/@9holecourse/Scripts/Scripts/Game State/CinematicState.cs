using UnityEngine;

public class CinematicState : SingletonBehaviour<CinematicState>, IGameState
{
    private Canvas canvas;
    private Cinematics cinematics;

    protected override void Awake()
    {
        base.Awake();

        GameStateManager.Instance.GameStates.Add(this.name, this);

        canvas = GetComponentInChildren<Canvas>();
        //cinematics = FindObjectOfType<Cinematics>(true);
        cinematics = CourseManager.Instance.ActiveCourse.transform.Find("Course Cinematics").GetComponent<Cinematics>();
        cinematics.OnSequenceComplete += OnCinematicSequenceComplete;

        Exit();
    }

    public void Enter()
    {
        canvas.gameObject.SetActive(true);
        cinematics.gameObject.SetActive(true);
    }

    public void Exit()
    {
        canvas.gameObject.SetActive(false);
        cinematics.gameObject.SetActive(false);
    }

    public void Process() { }

    private void OnCinematicSequenceComplete()
    {
        MatchManager.Instance.Initialize();
    }
}
