using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ShootingBehaviourState : AIBehaviourState
{
    ShotController shotController;

    public ShootingBehaviourState(Golfer golfer) : base(golfer) { }

    public async override void Enter()
    {
        shotController = GameObject.Find("ShotController").GetComponent<ShotController>();
        shotController.CheckTrajectory = true;
        await WaitBeforeProceeding(WaitState.PreDrag, 3000);
        await WaitBeforeProceeding(WaitState.PreShoot, 2000);
    }

    public override AIBehaviourState Process()
    {
        return null;
    }

    async Task WaitBeforeProceeding(WaitState state, int time)
    {
        await UniTask.Delay(time);
        if (state == WaitState.PreDrag)
            shotController.ProceedToDrag = true;
        else if (state == WaitState.PreShoot)
            shotController.ProceedToShoot = true;
    }

    private enum WaitState
    { 
        PreDrag,
        PreShoot
    }
}
