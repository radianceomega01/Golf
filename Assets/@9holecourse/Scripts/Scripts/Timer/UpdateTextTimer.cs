using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpdateTextTimer : MonoBehaviour
{
   [SerializeField] Timer timer;
    TextMeshProUGUI timerText;

    private void Awake()
    {
        timerText = GetComponentInChildren<TextMeshProUGUI>();
    }

    void Update()
    {
        timerText.text = ((int)timer.GetCurrentTime()).ToString();
    }
}
