
using Blueberry.Core.Data;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

// Contains match rules
public class MatchManager : SingletonBehaviour<MatchManager>
{
    [SerializeField] TextMeshProUGUI errorMsg;
    [SerializeField] Timer timer;

    private List<Golfer> golfers = new List<Golfer>();
    private int activeGolferIndex = -1;
    private float timerTime = 180;
    private CustomInstantiator customInstantiator;

    // Place from which game starts
    private Transform teeing;
    private int playersReady;

    //For Match Rules.
    public int BestCompletionShots
    {
        private set;
        get;
    }

    public int UserId 
    {
        private set;
        get;
    }

    public event Action<Golfer> OnOpponentAdded;

    public event Action<Golfer> OnGolferRemoved;
    public List<Golfer> GetAllGolfers() => golfers;
    public bool IsUserGolfer => activeGolferIndex == UserId - 1;

    public Timer GetTimer() => timer;

    private void OnEnable()
    {
        timer.OnTimerComplete += TimeOver;

        if (GameManager.Instance.GameMode != GameManager.GameModes.SinglePlayer)
        {
            customInstantiator = new CustomInstantiator();
            customInstantiator.OnEventRaised += InstantiateOthers;
            ConnectToServer.instance.OnPlayerLeft += RemovePlayerFromGame; 
            ConnectToServer.instance.OnPlayerConnected += RemoveError;
            ConnectToServer.instance.OnPlayerDisconnected += ShowError; 
        }  
    }

    public void Initialize()
    {
        teeing = CourseManager.Instance.ActiveCourse.teeing;

        InstantiateSelf();

        //enemyAI
        if (GameManager.Instance.GameMode == GameManager.GameModes.SinglePlayer)
        {
            InstantiateEnemy();
            activeGolferIndex = golfers.Count - 1;
            Proceed();
        }
    }

    private void InstantiateSelf()
    {
        Golfer golfer;
        int golferId;
        if (GameManager.Instance.GameMode == GameManager.GameModes.SinglePlayer)
        {
            golferId = golfers.Count + 1;
            golfer = Instantiate(MatchDetails.character.gameplayPrefab, teeing.position, teeing.rotation);
        }
        else
        {
            InstantiateOthers(CustomInstantiator.playerCache);
            int avatarId = MatchDetails.character.avatarId;
            int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
            golferId = golfers.Count + 1;
            string name;
            if (Session.User.Name.first == null || Session.User.Name.first == "")
                name = MatchDetails.character.name;
            else
                name = Session.User.Name.first;
            object[] specificData = new object[] { golferId, avatarId, name, actorNumber };

            var instantiatedObj = customInstantiator.Instantiate(MatchDetails.character.gameplayPrefab.gameObject, teeing.position, teeing.rotation, null, specificData);
            golfer = instantiatedObj.GetComponent<Golfer>();
            golfer.ActorNumber = actorNumber;
        }

        //userid and golferid for matchSummary
        //player
        ScoreCard playerCard = new ScoreCard();
        golfer.Scorecard = playerCard;
        golfer.Scorecard.Id = golferId;
        if (Session.User.Name.first == null || Session.User.Name.first == "")
            golfer.Scorecard.name = MatchDetails.character.name;
        else
            golfer.Scorecard.name = Session.User.Name.first;
        ScoreBoard.Instance.AddScoreCard(golfer.Scorecard.Id, playerCard);
        ScoreBoard.Instance.CreateGolferScore();
        UserId = golfer.Scorecard.Id;

        golfers.Add(golfer);
        golfer.gameObject.SetActive(false);

        if (GameManager.Instance.GameMode != GameManager.GameModes.SinglePlayer)
            PlayersReady();
    }

    private void InstantiateOthers(Queue playerQueue)
    {
        Golfer golferObj;

        while (playerQueue.Count > 0)
        {
            EventData playerData = (EventData)playerQueue.Dequeue();
            object[] data = (object[])playerData.CustomData;
            object[] specificData = (object[])data[1];
            Golfer instantiatingPrefab = CharacterList.Instance.GetCharacter((int)specificData[1]).gameplayPrefab;
            golferObj = Instantiate(instantiatingPrefab, (Vector3)data[2], (Quaternion)data[3]);

            PhotonView photonView = golferObj.GetComponent<PhotonView>();
            photonView.ViewID = (int)data[0];

            golferObj.ActorNumber = (int)specificData[3];
            ScoreCard playerCard = new ScoreCard();
            golferObj.Scorecard = playerCard;
            golferObj.Scorecard.Id = (int)specificData[0];
            golferObj.Scorecard.name = (string)specificData[2];

            ScoreBoard.Instance.CreateGolferScore();
            if (!ScoreBoard.Instance.GetScoreCards().ContainsKey(golferObj.Scorecard.Id))
                ScoreBoard.Instance.AddScoreCard(golferObj.Scorecard.Id, playerCard);

            golfers.Add(golferObj);
            OnOpponentAdded?.Invoke(golferObj);
            golferObj.gameObject.SetActive(false);
            PlayersReady();
        }
    }

    private void PlayersReady()
    {
        playersReady++;
        if (playersReady == PhotonNetwork.CurrentRoom.PlayerCount)
        {
            golfers = golfers.OrderBy(g => g.ActorNumber).ToList();
            UpdateUserId();
            activeGolferIndex = golfers.Count - 1;
            playersReady = 0;
            Proceed();
        }
    }

    private void UpdateUserId() // For multiplayer sync issue
    {
        for (int i = 0; i < golfers.Count; i++)
        {
            golfers[i].Scorecard.Id = i + 1;
            if (golfers[i].ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
                UserId = i + 1;
        }
    }

    //AI Enemy
    private void InstantiateEnemy()
    {
        var opponent = Instantiate(MatchDetails.opponent.gameplayPrefab, teeing.position, teeing.rotation);

        ScoreCard opponentCard = new ScoreCard();
        opponent.Scorecard = opponentCard;
        opponent.Scorecard.Id = golfers.Count + 1;
        opponent.Scorecard.name = MatchDetails.opponent.name;
        ScoreBoard.Instance.AddScoreCard(opponent.Scorecard.Id, opponentCard);
        ScoreBoard.Instance.CreateGolferScore();
        opponent.gameObject.AddComponent<GolferAI>().enabled = true;
        golfers.Add(opponent);
        OnOpponentAdded?.Invoke(opponent);
        opponent.gameObject.SetActive(false);
    }

    public void Proceed()
    {
        GetActiveGolfer().gameObject.SetActive(false);
        ExecuteMatchRules();
    }

    private void ExecuteMatchRules()
    {
        if (GetActiveGolfer().Scorecard.CourseComplete)
        {
            if (GetActiveGolfer().Scorecard.Shots < BestCompletionShots || BestCompletionShots == 0)
                BestCompletionShots = GetActiveGolfer().Scorecard.Shots;
        }

        if (!TryGetNextGolfer())
        {
            //GameComplete
            GameStateManager.Instance.SetState(MatchSummaryState.Instance.name);
            return;
        }

        if (BestCompletionShots > 0)
        {
            if (GetActiveGolfer().Scorecard.Shots + 1 <= CourseManager.Instance.ActiveCourse.maxAllowedShots || GetActiveGolfer().Scorecard.Shots + 1 <= BestCompletionShots)
            {
                GoToDirectingState();
                return;
            }
            else
            {
                //GameComplete
                GameStateManager.Instance.SetState(MatchSummaryState.Instance.name);
                return;
            }
        }
        GoToDirectingState();
    }

    private bool TryGetNextGolfer(int iterations = 0)
    {
        if (iterations >= golfers.Count)
        {
            return false;
        }
            
        activeGolferIndex++;
        if (activeGolferIndex >= golfers.Count)
            activeGolferIndex = 0;

        if (!GetActiveGolfer().Scorecard.CourseComplete && GetActiveGolfer().Scorecard.Shots + 1 <= CourseManager.Instance.ActiveCourse.maxAllowedShots && GetActiveGolfer()!= null)
            return true;
        else
            return TryGetNextGolfer(++iterations);
    }

    private void GoToDirectingState()
    {
        GetActiveGolfer().gameObject.SetActive(true);
        timer.ResetTimer();
        timer.StartTimer(timerTime);
        GameStateManager.Instance.SetState(DirectingState.Instance.name);
    }

    public Golfer GetActiveGolfer()
    {
        if(activeGolferIndex != -1)
            return golfers[activeGolferIndex];
        else 
            return null;
    }

    public Golfer GetGolferWithId(int id)
    {
        return golfers[id -1];
    }

    private void TimeOver()
    {
        timer.StopTimer();
        Proceed();
    }

    public MatchResult EvaluateMatchResult()
    {
        if (GetGolferWithId(UserId).Scorecard.CourseComplete == true && 
            GetGolferWithId(UserId).Scorecard.Shots == BestCompletionShots)
            return MatchResult.Won;
        else
            return MatchResult.Lost;
        
    }

    private void RemoveError()
    {
        errorMsg.gameObject.SetActive(false);
        errorMsg.text = "";
    }

    private void ShowError(DisconnectCause cause)
    {
        errorMsg.text = "Disconnected due to " + cause.ToString() + "!";
        errorMsg.gameObject.SetActive(true);
    }
    private void RemovePlayerFromGame(Player player)
    {
        Golfer leftGolfer = null;
        foreach (Golfer golfer in golfers)
        {
            if (golfer.ActorNumber == player.ActorNumber)
            { 
                leftGolfer = golfer;
                break;
            }
        }

        if (leftGolfer == null)
            return;

        if (leftGolfer == GetActiveGolfer())
        {
            TryGetNextGolfer();
            GoToDirectingState();
        }

        Destroy(leftGolfer.gameObject);
        OnGolferRemoved.Invoke(leftGolfer);
        ScoreBoard.Instance.RemoveScoreCard(leftGolfer.Scorecard.Id);
        ScoreBoard.Instance.RemoveGolferScore(leftGolfer.Scorecard.Id);
        //golfers.Remove(leftGolfer);

    }

    private void OnDisable()
    {
        if (GameManager.Instance.GameMode != GameManager.GameModes.SinglePlayer)
        {
            customInstantiator.OnEventRaised -= InstantiateOthers;
            ConnectToServer.instance.OnPlayerLeft -= RemovePlayerFromGame;
            ConnectToServer.instance.OnPlayerConnected -= RemoveError;
            ConnectToServer.instance.OnPlayerDisconnected -= ShowError;
        }
    }
    private void OnDestroy()
    {
        //GetActiveGolfer().Scorecard.Clear();
        ScoreBoard.Instance.Clear();
        timer.OnTimerComplete -= TimeOver;
    }
}
