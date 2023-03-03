using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TimerScript : MonoBehaviour
{
    public static TimerScript instance;
    [HideInInspector]
    public float maxTime = 10f;
    public float timeLeft;
    public UnityEvent onTimeUp;
    public float blinkTime = 5.0f;
    public bool startTimer;
    public Image TimerBar;
    public Gradient gradient;
    public Color defaultColour;

    private void Awake()
    {
        instance = this;
        TimerBar = GetComponent<Image>();
        defaultColour = TimerBar.color;
    }

    void Start()
    {     
        timeLeft = maxTime;
    }

    
    void Update()
    {
        if (startTimer)
        {
            if (timeLeft > 0)
            {   
                timeLeft -= Time.deltaTime;
                TimerBar.fillAmount = Mathf.Lerp(0, 1, timeLeft / maxTime);
                colourChanger();
            }
            else
            {
                onTimeUp.Invoke();
            }
        }

    }
    void colourChanger ()
    {
        Color clr = gradient.Evaluate(TimerBar.fillAmount);
        
        if (blinkTime > timeLeft)
        {
            clr.a = Mathf.PingPong(Time.time * 5, 1);
        }
        TimerBar.color = clr;
    }

    public void SetTime(float time)
    {
        timeLeft = time;
        maxTime = time;
    }

    public void SetStatusOfTimeBar(bool Status)
    {
        startTimer = Status;
    }

    public void SetDefaultGreen()
    {
        timeLeft = maxTime;
        TimerBar.color = defaultColour;
        TimerBar.fillAmount = 1;
    }

    
}

