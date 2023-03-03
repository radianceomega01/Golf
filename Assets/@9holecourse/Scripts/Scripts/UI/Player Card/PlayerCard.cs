using Blueberry.Core.Data;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCard : MonoBehaviour
{
    [SerializeField] private GameObject avatarIcon;
    [SerializeField] private TextMeshProUGUI avatarName;
    [SerializeField] private TextMeshProUGUI shotCount;

    Golfer golfer;
    Ball ball;

    // Start is called before the first frame update
    void Start()
    {
        golfer = MatchManager.Instance.GetGolferWithId(MatchManager.Instance.UserId);
        ball = golfer.Ball;
        ball.OnStateChanged += UpdateShotCount;

        avatarIcon.GetComponent<Image>().sprite = MatchDetails.character.avatarIcon;
        if (Session.User.Name.first == null || Session.User.Name.first == "")
            avatarName.text = MatchDetails.character.name;
        else
            avatarName.text = Session.User.Name.first;
        shotCount.text = golfer.Scorecard.Shots.ToString();
    }

    private void UpdateShotCount(Ball.State state, string courseArea)
    {
        shotCount.text = MatchManager.Instance.GetGolferWithId(MatchManager.Instance.UserId).Scorecard.Shots.ToString();
    }

    private void OnDestroy()
    {
        ball.OnStateChanged -= UpdateShotCount;
    }

}
