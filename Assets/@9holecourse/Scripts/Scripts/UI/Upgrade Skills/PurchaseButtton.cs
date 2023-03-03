
using UnityEngine;
using UnityEngine.UI;

public class PurchaseButtton : MonoBehaviour
{
    [SerializeField] private PurchaseType purchaseType;
    //[SerializeField] private GameObject purchasePopup;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(purchaseCoins);
    }

    private void purchaseCoins()
    {
        int amount = 0;
        int reduction = 0;
        switch (purchaseType)
        {
            case PurchaseType.low:
                amount += 10;
                reduction = 5;
                break;
            case PurchaseType.medium:
                amount += 25;
                reduction = 10;
                break;
            case PurchaseType.good:
                amount += 50;
                reduction = 20;
                break;
            case PurchaseType.high:
                amount += 100;
                reduction = 40;
                break;
        }
        PurchaseHandler.Instance.SpendBucks(amount, reduction);
    }

    public enum PurchaseType
    {
        low,
        medium,
        good,
        high,
    }
}
