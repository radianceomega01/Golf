using Photon.Pun;
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ClubSelectionSystem : MonoBehaviour
{
    [SerializeField] private ClubOption prefab;
    [SerializeField] private Button selectedClub;
    [SerializeField] private GameObject parentLayout;

    public event Action<Club> OnClubChanged;

    private GridLayoutGroup layoutGroup;
    private PhotonView photonView;

    private void Awake()
    {
        layoutGroup = GetComponentInChildren<GridLayoutGroup>();
        photonView = GetComponent<PhotonView>();
        ToggleParentLayout(false);
    }

    private IEnumerator Start()
    {
        SpawnClubOptions();
        //SelectClub(ClubList.Instance.GetClub(0));
        yield return new WaitForFixedUpdate();
    }

    private void SpawnClubOptions()
    {
        foreach (var club in ClubList.Instance.GetAllClubs())
        {
            var clubOption = Instantiate(prefab, layoutGroup.transform);
            clubOption.SetClub(club);
        }
    }

    public void SelectClub(Club club)
    {
        MatchManager.Instance.GetActiveGolfer().Club = club;
        selectedClub.image.sprite = club.selectedClubIcon;
        OnClubChanged?.Invoke(club);

        if (GameManager.Instance.GameMode != GameManager.GameModes.SinglePlayer)
        {
            photonView.RPC("SetClubToOthers", RpcTarget.Others, (int)club.type);
        }
    }

    [PunRPC]
    private void SetClubToOthers(int type)
    {
        Club club = ClubList.Instance.GetClub(type);
        MatchManager.Instance.GetActiveGolfer().Club = club;
        OnClubChanged?.Invoke(club);
    }
        
    public void ToggleParentLayout(bool value)
    {
        parentLayout.SetActive(value);
    }

}
