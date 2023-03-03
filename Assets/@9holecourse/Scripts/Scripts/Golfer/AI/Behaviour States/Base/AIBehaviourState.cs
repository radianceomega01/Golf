
public abstract class AIBehaviourState
{
    protected Golfer golfer;
    public AIBehaviourState(Golfer golfer)
    {
        this.golfer = golfer;
    }

    public abstract void Enter();
    public abstract AIBehaviourState Process();
}
