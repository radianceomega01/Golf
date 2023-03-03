using System;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public event Action OnTimerComplete;

    private float waitingTime;
    private bool canExecute;
    private float currentTime;

    private void Start()
    {
        currentTime = waitingTime;
    }

    void Update()
    {
        if (canExecute)
        {
            if (currentTime > 0)
            {
                currentTime -= Time.deltaTime;
            }
            else
            {
                canExecute = false;
                OnTimerComplete?.Invoke();
            }
        }
    }

    public float GetCurrentTime() => currentTime;

    public void ResetTimer() => currentTime = waitingTime;

    public void StartTimer(float value)
    {
        currentTime = value;
        waitingTime = value;
        canExecute = true;
    }

    public void StopTimer() => canExecute = false;
}
