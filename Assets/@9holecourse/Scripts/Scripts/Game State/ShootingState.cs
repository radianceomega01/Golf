using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShootingState : SingletonBehaviour<ShootingState>, IGameState
{
    [Header("View")]
    [SerializeField] private Canvas view;
    [SerializeField] private GameObject driverInputJoystick;
    [SerializeField] private GameObject chiperInputJoystick;
    [SerializeField] private GameObject ironInputJoystick;
    [SerializeField] private GameObject puterInputJoystick;
    [SerializeField] private GameObject inputPanel;
    [SerializeField] private new TargetCamera camera;
    [SerializeField] private Button reset;

    [Header("Component")]
    [SerializeField] private ShotController shotController;

    private Club club;
    private Golfer golfer;
    protected override void Awake()
    {
        base.Awake();

        GameStateManager.Instance.GameStates.Add(this.name, this);

        reset.onClick.AddListener(GoToDirectingState);
        Exit();
    }

    public void Enter()
    {
        Trajectory.Instance.SetWidthMultiplier(0.1f);

        golfer = MatchManager.Instance.GetActiveGolfer();

        view.gameObject.SetActive(true);
        camera.gameObject.SetActive(true);
        shotController.gameObject.SetActive(true);

        camera.SetTargetPosition(golfer.transform.position);
        camera.SetTargetRotaion(golfer.transform.rotation);
        club = MatchManager.Instance.GetActiveGolfer().Club;

        if (MatchManager.Instance.IsUserGolfer)
            EnableInputJoyStick();
    }

    public void Exit()
    {
        Trajectory.Instance.Clear();
        shotController.Reset();
        view.gameObject.SetActive(false);
        camera.gameObject.SetActive(false);
        shotController.gameObject.SetActive(false);
        DisableInputJoySticks();
    }

    private void EnableInputJoyStick()
    {
        if (club.type == Club.Type.Driver)
        {
            driverInputJoystick.SetActive(true);
            shotController.SetClubInput(driverInputJoystick.GetComponent<ClubInput>());
        }
        else if (club.type == Club.Type.Chiper)
        {
            chiperInputJoystick.SetActive(true);
            shotController.SetClubInput(chiperInputJoystick.GetComponent<ClubInput>());
        }
        else if (club.type == Club.Type.Irons)
        {
            ironInputJoystick.SetActive(true);
            shotController.SetClubInput(ironInputJoystick.GetComponent<ClubInput>());
        }
        else if (club.type == Club.Type.Puter)
        {
            puterInputJoystick.SetActive(true);
            shotController.SetClubInput(puterInputJoystick.GetComponent<ClubInput>());
        }
    }

    private void DisableInputJoySticks()
    {
        driverInputJoystick.SetActive(false);
        chiperInputJoystick.SetActive(false);
        ironInputJoystick.SetActive(false);
        puterInputJoystick.SetActive(false);
    }

    public void Process() 
    {
        if (MatchManager.Instance.IsUserGolfer)
            inputPanel.SetActive(true);
        else
            inputPanel.SetActive(false);
    }

    private void GoToDirectingState()
    {
        GameStateManager.Instance.SetState(DirectingState.Instance.name);
    }

}