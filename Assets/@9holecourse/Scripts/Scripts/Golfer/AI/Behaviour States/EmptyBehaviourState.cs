using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyBehaviourState : AIBehaviourState
{
    public EmptyBehaviourState(Golfer golfer) : base(golfer) { }

    public override void Enter()
    {
        
    }

    public override AIBehaviourState Process()
    {
        return null;
    }
}
