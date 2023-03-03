using System;
using Cinemachine;
using UnityEngine;

public class DollySystem : MonoBehaviour
{
    private CinemachineDollyCart cart;
    private bool complete;

    public event Action OnComplete; 

    private void Awake()
    {
        cart = GetComponentInChildren<CinemachineDollyCart>();
        cart.m_PositionUnits = CinemachinePathBase.PositionUnits.Normalized;
    }

    private void OnEnable()
    {
        cart.m_Position = 0;
        complete = false;
    }

    private void Update()
    {
        if (!complete && cart.m_Position == 1)
        {
            complete = true;
            OnComplete?.Invoke();
        }
    }
}
