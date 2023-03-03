using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotController : MonoBehaviour
{
    private ClubInput clubInput;

    private Vector3 force;
    private Ball ball;
    private float minForce = 0.3f;
    private float nerfedForce = 0.6f;
    private float dragTime;
    private bool reduceForce;

    //characterSkills
    private float characterForce;
    private float characterDriveInacc;
    private float characterPuttInacc;
    private float characterLuck;

    public bool ProceedToDrag
    {
        get;
        set;
    }

    public bool ProceedToShoot
    {
        get;
        set;
    }

    public bool CheckTrajectory
    {
        get;
        set;
    }

    private void OnEnable()
    {
        if (MatchManager.Instance.GetActiveGolfer() != null)
        { 
            ball = MatchManager.Instance.GetActiveGolfer().Ball;
            characterForce = MatchManager.Instance.GetActiveGolfer().GetCharacterStatValues().drives;
            characterDriveInacc = MatchManager.Instance.GetActiveGolfer().GetCharacterStatValues().inaccuracy;
            characterPuttInacc = MatchManager.Instance.GetActiveGolfer().GetCharacterStatValues().inaccPutts;
            characterPuttInacc = MatchManager.Instance.GetActiveGolfer().GetCharacterStatValues().inaccPutts;
            characterLuck = MatchManager.Instance.GetActiveGolfer().GetCharacterStatValues().luck;
        }
    }

    private void OnDisable()
    {
        if(clubInput !=  null)
            clubInput.OnDragReleased -= Shoot;
    }
    
    void Update()
    {
        if (ball == null)
            return;

        if (clubInput != null)
        {
            if (clubInput.isDraging && clubInput.GetDragInputAxis().force > 0.1f)
            {
                if (MatchManager.Instance.GetActiveGolfer().Club.type != Club.Type.Puter)
                    MatchManager.Instance.GetActiveGolfer().SetAnimationState(Golfer.AnimationState.DriveRaise, clubInput.GetDragInputAxis());

                force = MatchManager.Instance.GetActiveGolfer().GetForce(clubInput.GetDragInputAxis());
                int layerMask = LayerMask.GetMask(LayerMask.LayerToName(0));
                Trajectory.Instance.Draw(ball.transform.position, force, out RaycastHit hit, layerMask);
            }

            if (clubInput.GetDragInputAxis().force < minForce)
                Trajectory.Instance.Clear();
        }
        else
        {
            float aiForce = 0;
            if (CheckTrajectory)
            {
                force = MatchManager.Instance.GetActiveGolfer().GetForce(InputAxis.Default());
                int layerMask = LayerMask.GetMask(LayerMask.LayerToName(3));
                Trajectory.Instance.CreateRaycast(ball.transform.position, force, out RaycastHit hit, layerMask);
                if ((hit.transform.position - ball.transform.position).magnitude > 
                    (CourseManager.Instance.ActiveCourse.hole.position - MatchManager.Instance.GetActiveGolfer().transform.position).magnitude
                    && MatchManager.Instance.GetActiveGolfer().Club.type != Club.Type.Puter)
                {
                    reduceForce = true;
                }
            }
            if (ProceedToDrag)
            {
                dragTime += Time.deltaTime * 1.5f;
                if (reduceForce)
                {
                    aiForce = Mathf.Lerp(0f, InputAxis.CustomInput(nerfedForce, 0f).force, dragTime);
                }
                else
                    aiForce = Mathf.Lerp(0f, InputAxis.Default().force, dragTime);

                if(MatchManager.Instance.GetActiveGolfer().Club.type != Club.Type.Puter)
                    MatchManager.Instance.GetActiveGolfer().SetAnimationState(Golfer.AnimationState.DriveRaise, InputAxis.CustomInput(aiForce, 0f));
            
                force = MatchManager.Instance.GetActiveGolfer().GetForce(InputAxis.CustomInput(aiForce, 0f));
                int layerMask = LayerMask.GetMask(LayerMask.LayerToName(0));
                Trajectory.Instance.Draw(ball.transform.position, force, out RaycastHit hit, layerMask);

            }
            if (ProceedToShoot)
            {
                ProceedToDrag = false;
                InputAxis customInputAxis = InputAxis.CustomInput(aiForce, 0f);
                Shoot(customInputAxis);
                ProceedToShoot = false;
                CheckTrajectory = false;
                reduceForce = false;
                dragTime = 0f;
            }
                
        }
    }

    public void Shoot(InputAxis inputAxis)
    {
        if (inputAxis.force > minForce)
        {
            InputAxis abilityInputAxis = AddCharacterAbility(inputAxis, MatchManager.Instance.GetActiveGolfer().Club.type);

            if (MatchManager.Instance.GetActiveGolfer().Club.type != Club.Type.Puter)
            { 
                if(!IsLucky(characterLuck))
                    MatchManager.Instance.GetActiveGolfer().SetAnimationState(Golfer.AnimationState.DriveRelease, abilityInputAxis);
                else
                    MatchManager.Instance.GetActiveGolfer().SetAnimationState(Golfer.AnimationState.DriveRelease, abilityInputAxis);
            }
                
            else
                MatchManager.Instance.GetActiveGolfer().SetAnimationState(Golfer.AnimationState.Putt, abilityInputAxis);

            Trajectory.Instance.Clear();
            clubInput = null;
            GameStateManager.Instance.SetState(BallViewState.Instance.name);
        }
    }

    private InputAxis AddCharacterAbility(InputAxis inputAxis, Club.Type type)
    {
        inputAxis.force *= characterForce;

        if (type == Club.Type.Puter)
            inputAxis.angle += characterPuttInacc;
        else
            inputAxis.angle += characterDriveInacc;

        return inputAxis;
    }

    private bool IsLucky(float luck)
    {
        if (Random.Range(0f, 1f) <= luck)
            return true;
        else
            return false;
    }

    public ClubInput GetClubInput() => clubInput;
    public void SetClubInput(ClubInput clubInput)
    {
        this.clubInput = clubInput;
        if(clubInput != null)
            clubInput.OnDragReleased += Shoot;
    }

    public void Reset()
    {
        if (clubInput != null)
        {
            clubInput.Reset();
            clubInput = null;
        }
    } 
}
