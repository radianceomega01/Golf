using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindManager : SingletonBehaviour<WindManager>
{
    [SerializeField] private float windForce;

    public event Action<Vector3> OnWindChanged;

    private Vector3 wind = Vector3.zero;
    private float characterWindInacc;
    private PhotonView photonView;
    public Vector3 Wind => wind;

    protected override void Awake()
    {
        characterWindInacc = MatchDetails.character.statValues.inaccWinds;
        photonView = GetComponent<PhotonView>();
    }
    public void RandomizeWind()
    {
        windForce *= UnityEngine.Random.Range(0.8f, 1.2f);
        wind = new Vector3(UnityEngine.Random.Range(-1, 2), 0, UnityEngine.Random.Range(-1, 2)) * (windForce + characterWindInacc);
        //wind.forward = MatchManager.Instance.GetActiveGolfer().transform.forward;
        OnWindChanged?.Invoke(wind);

        if (GameManager.Instance.GameMode != GameManager.GameModes.SinglePlayer)
            photonView.RPC("SetWindForOthers", RpcTarget.Others, wind);
    }

    [PunRPC]
    private void SetWindForOthers(Vector3 wind)
    {
        this.wind = wind;
        OnWindChanged?.Invoke(wind);
    }
}
