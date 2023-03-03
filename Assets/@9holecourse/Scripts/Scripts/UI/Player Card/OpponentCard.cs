using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OpponentCard : MonoBehaviour
{
    [SerializeField] private GameObject avatarIcon;
    [SerializeField] private TextMeshProUGUI avatarName;
    [SerializeField] private TextMeshProUGUI shotCount;

    private Golfer golfer;
    private Ball ball;
    public int UserId { get; private set; }

    public void SetGolfer(Golfer golfer)
    {
        this.golfer = golfer;
        UserId = golfer.Scorecard.Id;
        avatarIcon.GetComponent<Image>().sprite = golfer.GetCharacter().avatarIcon;
        avatarName.text = golfer.Scorecard.name;
        shotCount.text = golfer.Scorecard.Shots.ToString();

        ball = golfer.Ball;
        ball.OnStateChanged += UpdateShotCount;
    }

    private void UpdateShotCount(Ball.State state, string courseArea)
    {
        shotCount.text = golfer.Scorecard.Shots.ToString();
    }

    private void OnDestroy()
    {
        ball.OnStateChanged -= UpdateShotCount;
    }
}
