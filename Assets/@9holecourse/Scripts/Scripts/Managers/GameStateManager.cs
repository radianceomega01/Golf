using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : SingletonBehaviour<GameStateManager>
{
    private IGameState gameState;
    private PhotonView photonView;
    private Dictionary<string, IGameState> gameStates = new Dictionary<string, IGameState>();

    public event Action<IGameState> OnStateChanged;
    public Dictionary<string, IGameState> GameStates { get => gameStates; set => gameStates = value; }

    protected override void Awake() => photonView = GetComponent<PhotonView>();
    private void Start()
    { 
        //if(GameManager.Instance.GameMode == GameManager.GameModes.SinglePlayer)
            SetState(CinematicState.Instance.name);
    }

    private void Update()
    {
        if (gameState == null)
        {
            return;
        }
        gameState.Process();
    }

    public void SetState(string stateName)
    {
        IGameState previousState = gameState;

        if (this.gameState != null)
            this.gameState.Exit();
        this.gameState = GameStates[stateName];

        this.gameState.Enter();
        OnStateChanged?.Invoke(gameState);

        if (GameManager.Instance.GameMode != GameManager.GameModes.SinglePlayer)
        {
            if (previousState == GameStates[DirectingState.Instance.name] ||
                previousState == GameStates[ShootingState.Instance.name])
                photonView.RPC("syncState", RpcTarget.Others, stateName);
        }
    }

    [PunRPC]
    private void syncState(string stateName)
    {
        if (gameState == GameStates[stateName] || gameState == null)
            return;

        gameState.Exit();
        gameState = GameStates[stateName];
        gameState.Enter();
    }

}
