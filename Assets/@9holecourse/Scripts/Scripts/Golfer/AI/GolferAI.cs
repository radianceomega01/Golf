using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GolferAI : MonoBehaviour
{
    Golfer golfer;
    AIBehaviourState state;
    AIBehaviourState currentState;
    GameStateManager gameStateManager;
    void Awake()
    {
        golfer = gameObject.GetComponent<Golfer>();
        gameStateManager = GameStateManager.Instance;
        gameStateManager.OnStateChanged += ChangeAIBehaviourState;
    }

    void Update()
    {
        if(currentState != null)
            currentState.Process();
        /*if(state == null)
            return;
        currentState = state;
        currentState.Enter();*/
    }

    private void ChangeAIBehaviourState(IGameState gameState)
    {
        if (!MatchManager.Instance.IsUserGolfer)
        {
            if (gameState is DirectingState)
                currentState = new DirectingBehaviourState(golfer);
            else if (gameState is ShootingState)
                currentState = new ShootingBehaviourState(golfer);
            else if (gameState is BallViewState)
                currentState = new EmptyBehaviourState(golfer);
            else if (gameState is CinematicState)
                currentState = new EmptyBehaviourState(golfer);

            currentState.Enter();
        }
    }

    private void OnDestroy()
    {
        gameStateManager.OnStateChanged -= ChangeAIBehaviourState;
    }
}
