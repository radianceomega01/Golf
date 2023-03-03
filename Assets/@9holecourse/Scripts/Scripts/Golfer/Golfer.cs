using Photon.Pun;
using System;
using UnityEngine;

public class Golfer : MonoBehaviour
{
    public enum State
    { 
        Playing,
        Analyzing
    }

    public enum AnimationState
    {
        Idle,
        Putt,
        DriveRaise,
        DriveRelease,
        Win,
        Loose
    }

    public event Action<State> OnStateChanged;
    public event Action<Club> OnClubChanged;

    [SerializeField] private GameObject character;
    [SerializeField] private Transform clubHolder;
    [SerializeField] private Character characterData;

    [Header("Sand Particles")]
    [SerializeField] private GameObject sandParticles;

    private Ball ball;
    private Club club;
    private State state;
    private Animator animator;
    private InputAxis inputAxis;
    public PhotonView PhotonView { get; private set; }
    public int ActorNumber { get; set; } //For multiplayer
    //For Scoreboard
    private ShotData shotData;
    public ScoreCard Scorecard
    {
        get;
        set;
    }

    public Club Club
    {
        get => club;
        set
        {
            club = value;
            var clubObject = ClubHolder.Find(club.name);
            if (clubObject == null)
            {
                Instantiate(club.prefab, ClubHolder).name = club.name;
            }

            foreach (Transform child in ClubHolder)
            {
                //Debug.Log(child.name);
                child.gameObject.SetActive(child.name == club.name);
            }

            OnClubChanged?.Invoke(club);
        }
    }

    public Ball Ball => ball;

    public Transform ClubHolder => clubHolder;

    private void Awake()
    {
        ball = GetComponentInChildren<Ball>();
        ball.OnStateChanged += OnBallStateChanged;
        animator = GetComponentInChildren<Animator>();
        PhotonView = GetComponentInChildren<PhotonView>();
    }

    private void OnEnable()
    {
        if (!ball.gameObject.activeInHierarchy)
        {
            ball.gameObject.SetActive(true);
        }

        SetState(State.Playing);
    }

    private void OnDisable()
    {
        var position = ball.transform.position;
        transform.position = position;
        ball.transform.position = transform.position;
        ball.transform.rotation = Quaternion.Euler(Vector3.zero);
    }

    private void Start()
    {
        club = ClubList.Instance.GetClub(0);
    }

    public void Shoot()
    {
        Vector3 resultantForce = GetForce(inputAxis);

        if (Ball.GetComponent<SphereCollider>().sharedMaterial == Ball.GetHazardMaterial() && club.type != Club.Type.Chiper)
        {
            Instantiate(sandParticles, Ball.transform.position, Quaternion.identity);
            resultantForce.y = resultantForce.y / 5;
            resultantForce.x = resultantForce.x / 5;
            resultantForce.z = resultantForce.z / 5;
        }
        ball.Shoot(resultantForce);
    }

    public Character.StatValues GetCharacterStatValues() => characterData.statValues;

    public Character GetCharacter() => characterData;

    public Vector3 GetForce(InputAxis axis)
    {
        Vector3 resultantForce;
        resultantForce = new Vector3(0f, club.force.upward, club.force.forward) * axis.force;
        resultantForce = -resultantForce.RotateAroundPointByAngle(Vector3.zero, transform.eulerAngles);
        resultantForce = -resultantForce.RotateAroundPointByAngle(Vector3.zero, Vector3.up * axis.angle * 30);
        return resultantForce;
    }

    public void SetAnimationState(AnimationState state, params object[] args)
    {
        if(args.Length > 0)
            inputAxis = (InputAxis)args[0];

        if (state == AnimationState.Putt)
        {
            animator.SetFloat("Drag", inputAxis.force);

            animator.SetBool("IsPutt", true);
            animator.SetTrigger("Release");
        }
        else if (state == AnimationState.DriveRaise)
        {
            animator.SetFloat("Drag", inputAxis.force);
        }
        else if (state == AnimationState.DriveRelease)
        {
            animator.SetBool("IsPutt", false);
            animator.SetTrigger("Release");
            animator.SetFloat("Drag", 0);
        }

        else if (state == AnimationState.Win)
            animator.SetTrigger("Won");

        else if (state == AnimationState.Loose)
            animator.SetTrigger("Lost");

        if (GameManager.Instance.GameMode != GameManager.GameModes.SinglePlayer)
        {
            if (args.Length > 0)
                PhotonView.RPC("SetAnimationForOthers", RpcTarget.Others, state, inputAxis.force, inputAxis.angle);
            else
                PhotonView.RPC("SetAnimationForOthers", RpcTarget.Others, state, 0f, 0f);
        }
            
    }

    [PunRPC]
    public void SetAnimationForOthers(int state, float force, float angle)
    {
        inputAxis = InputAxis.CustomInput(force, angle);

        if ((AnimationState)state == AnimationState.Putt)
        {
            animator.SetFloat("Drag", inputAxis.force);

            animator.SetBool("IsPutt", true);
            animator.SetTrigger("Release");
        }
        else if ((AnimationState)state == AnimationState.DriveRaise)
        {
            animator.SetFloat("Drag", inputAxis.force);
        }
        else if ((AnimationState)state == AnimationState.DriveRelease)
        {
            animator.SetBool("IsPutt", false);
            animator.SetTrigger("Release");
            animator.SetFloat("Drag", 0);
        }

    }

    public void ToggleClubHolder(bool value)
    {
        clubHolder.gameObject.SetActive(value);
    }

    public void ToggleBall( bool value) => Ball.gameObject.SetActive(value);

    private void OnBallStateChanged(Ball.State ballState, string area)
    {
        int yardsTravelled = (int)(ball.transform.position - gameObject.transform.position).magnitude;
        shotData = new ShotData(area, yardsTravelled);
        if(ballState == Ball.State.Par)
            Scorecard.AddShotData(shotData, true);
        else
            Scorecard.AddShotData(shotData, false);
        /*if (scorecard.CourseComplete)
            ScoreBoard.Instance.AddScoreCard(MatchManager.Instance.UserId, scorecard);*/

        SetState(State.Analyzing);
    }

    private void SetState(State state)
    {
        this.state = state;
        OnStateChanged?.Invoke(state);
    }

}
