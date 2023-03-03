using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpdateTimer : MonoBehaviour
{
    public Image timerBar;
    public Gradient gradient;
    public Color defaultColour;
    public float totalTime;
    public Timer timer;

    private float blinkTime = 5.0f;

    private void Awake()
    {
        if (timer == null)
            timer = MatchManager.Instance.GetTimer();
    }

    void Start()
    {
        defaultColour = timerBar.color;
    }

    void Update()
    {
        timerBar.fillAmount = Mathf.Lerp(0, 1, timer.GetCurrentTime() / totalTime);
        colourChanger();
    }

    void colourChanger()
    {
        Color clr = gradient.Evaluate(timerBar.fillAmount);

        if (blinkTime > timer.GetCurrentTime())
        {
            clr.a = Mathf.PingPong(Time.time * 5, 1);
        }
        timerBar.color = clr;
    }
}
