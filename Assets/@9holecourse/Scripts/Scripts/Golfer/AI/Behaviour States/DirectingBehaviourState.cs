
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class DirectingBehaviourState : AIBehaviourState
{
    RaycastHit hit;
    float randomAngle;
    Transform from;
    float timeToRotate;
    int tries = 0;
    Ball ball;
    PhysicMaterial material;
    bool canProceed;
    bool angleChanged;
    float lerpValue;

    public DirectingBehaviourState(Golfer golfer) : base(golfer) { }

    public async override void Enter()
    {
        ball = golfer.Ball;
        material = ball.GetComponent<SphereCollider>().sharedMaterial;
        from = golfer.transform;
        randomAngle = from.localEulerAngles.y;

        await WaitBeforeProceeding(WaitType.starting, 1500);
        DrawTrajectory();
        SelectClub();
        CalculateGolferRotation();

        await WaitBeforeProceeding(WaitType.Switching, 3500);
    }

    public override AIBehaviourState Process()
    {
        if (angleChanged)
        {
            timeToRotate += Time.deltaTime/2;
            //golfer.transform.rotation = Quaternion.Lerp(from.localRotation, to.localRotation, timeToRotate);
            lerpValue = Mathf.Lerp(from.localEulerAngles.y, randomAngle, timeToRotate);
            golfer.transform.localEulerAngles = Vector3.up * lerpValue;
            DrawTrajectory();
        }

        if (canProceed)
        {
            Trajectory.Instance.Clear();
            GameStateManager.Instance.SetState(ShootingState.Instance.name);
            return new ShootingBehaviourState(golfer);
        }
           
        return null;
    }
     


    private void DrawTrajectory()
    {
        var force = golfer.GetForce(InputAxis.Default());
        int layerMask = LayerMask.GetMask(LayerMask.LayerToName(3));
        Trajectory.Instance.Draw(golfer.transform.position, force, out hit, layerMask);
    }

    private void CalculateGolferRotation()
    {
        float initialAngle = from.localEulerAngles.y;
        while (tries <= 12)
        {
            if (hit.collider.tag == "Tag 6" || hit.collider.tag == "Tag 7")
            {
                if (tries >= 0 && tries < 6)
                {
                    randomAngle += 15;
                }
                else if (tries == 6)
                {
                    randomAngle = initialAngle;
                }
                else if (tries > 6 && tries <= 12)
                {
                    randomAngle -= 15;
                }
                tries++;
                golfer.transform.eulerAngles = Vector3.up * randomAngle;
                DrawTrajectory();
            }
            else
            {
                break;
            }
        }
        golfer.transform.localEulerAngles = Vector3.up * initialAngle;
        angleChanged = true;
    }
    
    
    private void SelectClub()
    {
        if (material.name == "Green")
            golfer.Club = ClubList.Instance.GetClub(3);
        else if (material.name == "Rough" || material.name == "Fairway")
            golfer.Club = ClubList.Instance.GetClub(2);
        else if (material.name == "Hazard")
            golfer.Club = ClubList.Instance.GetClub(1);
        else if (material.name == "Default")
            golfer.Club = ClubList.Instance.GetClub(0);

        return;
    }

    async Task WaitBeforeProceeding(WaitType type, int time)
    {
        await UniTask.Delay(time);

        if(type == WaitType.Switching)
            canProceed = true;
    }

    private enum WaitType
    { 
        starting,
        Switching
    }
}
