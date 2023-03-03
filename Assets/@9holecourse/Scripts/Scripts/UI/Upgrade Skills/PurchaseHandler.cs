using Blueberry.Core.API.Game;
using Blueberry.Core.API.Score;
using Blueberry.Core.Data;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PurchaseHandler : SingletonBehaviour<PurchaseHandler>
{
    [SerializeField] private TextMeshProUGUI coins;
    [SerializeField] private TextMeshProUGUI bucks;
    [SerializeField] private GameObject purchasePopup;
    [SerializeField] private Button purchase;
    [SerializeField] private TextMeshProUGUI errorText;
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private Image loadingBar;
    public static int Coins { get; set; }
    public static int Bucks { get; private set; }

    public event Action OnCoinsUpdated;

    protected override void Awake()
    {
        purchase.onClick.AddListener(EnablePopup);
    }

    private void Start()
    {
        GetBucks();
        UpdateCoinsOnScreen();
    }

    private void Update()
    {
        if (loadingPanel.activeInHierarchy)
            loadingBar.fillAmount += 0.01f;
    }
    public void UpdateCoinsOnScreen()
    {
        coins.text = Coins.ToString();
        DisablePopup();
        OnCoinsUpdated.Invoke();
    }

    public void GetBucks()
    {
        Bucks = (int)float.Parse(Session.User.blueberryBucks);
        bucks.text = Bucks.ToString();
    }

    public async void SpendBucks(int deltaCoins, int reduction)
    {
        loadingPanel.SetActive(true);
        JObject response = await RedeemBlueberryBucksPoster.PostRedeem(reduction);
        if (!(bool)response["status"])
            ShowErrorMsg((string)response["message"]);
        else
        {
            Bucks -= reduction;
            bucks.text = Bucks.ToString();
            PostUpdatedCoins(deltaCoins);
        }

        loadingBar.fillAmount = 0;
        loadingPanel.SetActive(false);
    }

    private async void PostUpdatedCoins(int deltaCoins)
    {
        int count = CharacterList.Instance.GetAllCharacters().Length;
        GameData gameData = GameData.Instance;

        for (int i = 0; i < count; i++)
        {
            Character character = CharacterList.Instance.GetCharacter(i);
            StatsData statsData = new StatsData();
            statsData.SetData(character.stats.luck, character.stats.power, character.stats.accuracy,
                character.stats.drives, character.stats.winds, character.stats.putts);

            StatsValueData statsValueData = new StatsValueData();
            statsValueData.SetData(character.statValues.luck, character.statValues.inaccuracy,
                character.statValues.drives, character.statValues.inaccPutts, character.statValues.inaccWinds);

            gameData.AddStatsData(i, statsData);
            gameData.AddStatsValueData(i, statsValueData);
        }

        gameData.SetCoins(Coins + deltaCoins);

        string dataToSend = JsonConvert.SerializeObject(gameData);
        JObject response = await UserGameSkillPoster.PostGameSkill(dataToSend);

        if (!(bool)response["status"])
            ShowErrorMsg("Failed to update coins!");
        else
        {
            Coins += deltaCoins;
            UpdateCoinsOnScreen();
        }
            DisablePopup();

        loadingBar.fillAmount = 0;
        loadingPanel.SetActive(false);
    }

    private void EnablePopup() => purchasePopup.SetActive(true);
    public void DisablePopup() => purchasePopup.SetActive(false);

    private async void ShowErrorMsg(string msg)
    {
        errorText.text = msg;
        errorText.gameObject.SetActive(true);
        await UniTask.Delay(2500);
        errorText.gameObject.SetActive(false);
    }
}
