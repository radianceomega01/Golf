
using Photon.Pun;
using System.Collections;
using System.Threading.Tasks;
using System.Timers;
using UnityEngine;
using UnityEngine.UI;

public class BallViewState : SingletonBehaviour<BallViewState>, IGameState
{
    [Header("View")]
    [SerializeField] private new TargetCamera camera;
    [SerializeField] private Canvas view;
    [SerializeField] private GameObject ballStats;
    [SerializeField] private Text yardsTravelled;
    [SerializeField] private Text ballResult;
    [SerializeField] private Text yardsLeft;

    private Ball ball;
    private Vector3 ballStartingPos;
    private int yardsTravelledValue;
    private int yardsLeftValue;
    private float yardsConverter;
    private Timer timer;
    private PhotonView photonView;
    private int playersReached;
    protected override void Awake()
    {
        base.Awake();
        photonView = GetComponent<PhotonView>();
        GameStateManager.Instance.GameStates.Add(this.name, this);
        timer = MatchManager.Instance.GetTimer();
        Exit();
        yardsConverter = 1.1f;
    }

    public void Enter()
    {
        ball = MatchManager.Instance.GetActiveGolfer().Ball;
        ballStartingPos = ball.transform.position;
        ballResult.text = "";
        yardsLeft.text = "";

        timer.StopTimer();

        ball.OnStateChanged += UpdateResult;

        camera.gameObject.SetActive(true);
        camera.SetTargetPosition(ball.transform.position);
        camera.SetTargetRotaion(MatchManager.Instance.GetActiveGolfer().transform.rotation);

        view.gameObject.SetActive(true);
    }

    public void Exit()
    {
        if (ball != null)
            ball.OnStateChanged -= UpdateResult;

        playersReached = 0;
        camera.gameObject.SetActive(false);
        view.gameObject.SetActive(false);
        ballStats.gameObject.SetActive(false);
    }

    public void Process()
    {
        if(ball.GetState() != Ball.State.Par)
            camera.SetTargetPosition(ball.transform.position);
    }

    public void UpdateResult(Ball.State state, string area)
    {
        if (GameManager.Instance.GameMode != GameManager.GameModes.SinglePlayer)
            photonView.RPC("UpdatePlayersReached", RpcTarget.All);

        ballStats.SetActive(true);
        ballResult.text = area;
        yardsTravelledValue = (int)((ball.transform.position - ballStartingPos).magnitude * yardsConverter);
        yardsTravelled.text = yardsTravelledValue + " yards";
        yardsLeftValue = (int)((CourseManager.Instance.ActiveCourse.hole.position - ball.transform.position).magnitude * yardsConverter);
        yardsLeft.text = yardsLeftValue + " yards left";

        if(GameManager.Instance.GameMode == GameManager.GameModes.SinglePlayer)
            StartCoroutine(WaitingTask(3f));
    }

    [PunRPC]
    private void UpdatePlayersReached()
    {
        playersReached++;
        if (playersReached == PhotonNetwork.CurrentRoom.PlayerCount)
            StartCoroutine(WaitingTask(3f));
    }
    IEnumerator WaitingTask(float value)
    {
        yield return new WaitForSeconds(value);
        MatchManager.Instance.Proceed();
    }
}
