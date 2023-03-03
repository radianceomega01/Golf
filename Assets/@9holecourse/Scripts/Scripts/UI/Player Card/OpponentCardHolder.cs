
using UnityEngine;

public class OpponentCardHolder : MonoBehaviour
{
    [SerializeField] private GameObject opponentCardPrefab;

    private Golfer opponent;
    MatchManager matchManager;
    private void Awake()
    {
        matchManager = MatchManager.Instance;
        matchManager.OnOpponentAdded += AddGolferCard;
        matchManager.OnGolferRemoved += RemoveOpponentCard;
    }

    private void OnDestroy()
    {
        matchManager.OnOpponentAdded -= AddGolferCard;
        matchManager.OnGolferRemoved -= RemoveOpponentCard;
    }

    private void AddGolferCard(Golfer golfer)
    {
        if (golfer.PhotonView.IsMine)
            return;

        var newCard = Instantiate(opponentCardPrefab, gameObject.transform);
        var opponentCard = newCard.GetComponent<OpponentCard>();
        opponentCard.SetGolfer(golfer);
    }

    private void RemoveOpponentCard(Golfer golfer)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            var opponentCard = transform.GetChild(i).GetComponent<OpponentCard>();
            if(opponentCard.UserId == golfer.Scorecard.Id)
            {
                Destroy(opponentCard.gameObject);
                break;
            }
        }
    }

}
