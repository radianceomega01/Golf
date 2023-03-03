
using Blueberry.Core.API.Game;
using Blueberry.Core.Data;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillUpgrader : SingletonBehaviour<SkillUpgrader>
{
    [SerializeField] private TextMeshProUGUI skill;
    [SerializeField] private TextMeshProUGUI skillDescription;
    [SerializeField] private GameObject upgradePopup;
    [SerializeField] private Button incrementBtn;
    [SerializeField] private Button decrementBtn;
    [SerializeField] private Button confirm;
    [SerializeField] private TextMeshProUGUI errorText;
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private Image loadingBar;

    private Slider upgradeSlider;
    private int tempCoins;
    private int oldSliderValue;
    protected override void Awake()
    {
        incrementBtn.onClick.AddListener(UpGrade);
        decrementBtn.onClick.AddListener(DownGrade);
        confirm.onClick.AddListener(SetNewValues);
        PurchaseHandler.Instance.OnCoinsUpdated += UpdateTempCoins;
        UpdateTempCoins();
    }

    private void Update()
    {
        if (loadingPanel.activeInHierarchy)
            loadingBar.fillAmount += 0.01f;
    }
    private void UpdateTempCoins() => tempCoins = PurchaseHandler.Coins;

    public void EnablePopup() => upgradePopup.SetActive(true);
    public void DisablePopup() => upgradePopup.SetActive(false);

    public void OnCloseClicked()
    {
        UpdateTempCoins();
        upgradeSlider.value = oldSliderValue;
        DisablePopup();
    }
    public void SetValues(Slider slider, string skillName)
    {
        upgradeSlider = slider;
        oldSliderValue = (int)upgradeSlider.value;
        skill.text = skillName;
        string activeCharacterName = CharacterSelection.Instance.GetClickedButton().GetCharacterData().name;
        skillDescription.text = "Upgrade your "+ activeCharacterName + "'s " + skill.text + " here by using your game coins";
    }
    private void UpGrade()
    {
        tempCoins -= 5;
        if(tempCoins < 0)
        {
            tempCoins += 5;
            ShowErrorMsg("Insufficient coins!");
            return;
        }
        upgradeSlider.value += 2;
    }
    private void DownGrade()
    {
        if (upgradeSlider.value >= oldSliderValue + 2)
        {
            tempCoins += 5;
            upgradeSlider.value -= 2;
        }
    }
    private void SetNewValues()
    {
        if (upgradeSlider.value == oldSliderValue)
        {
            DisablePopup();
            return;
        }
        loadingPanel.SetActive(true);
        UpdateStatValues();
        UpdateStats();
        PostSkills();
        PurchaseHandler.Coins = tempCoins;
        PurchaseHandler.Instance.UpdateCoinsOnScreen();
    }

    private void UpdateStatValues()
    {
        Character.StatValues statValues = CharacterSelection.Instance.GetClickedButton().GetCharacterData().statValues;
        switch (skill.text.Trim())
        {
            case "Luck":
                statValues.luck += ((int)upgradeSlider.value - oldSliderValue)/100;
                break;
            /*case "Power":
                statValues.drives += ((int)upgradeSlider.value - oldSliderValue) / 100;
                break;*/
            case "Accuracy":
                if(statValues.inaccuracy < 0)
                    statValues.inaccuracy += ((int)upgradeSlider.value - oldSliderValue) / 100;
                else
                    statValues.inaccuracy -= ((int)upgradeSlider.value - oldSliderValue) / 100;
                break;
            case "Drives":
                statValues.drives += ((int)upgradeSlider.value - oldSliderValue) / 100;
                break;
            case "Winds":
                if (statValues.inaccWinds < 0)
                    statValues.inaccWinds += ((int)upgradeSlider.value - oldSliderValue) / 100;
                else
                    statValues.inaccWinds -= ((int)upgradeSlider.value - oldSliderValue) / 100;
                break;
            case "Putts":
                statValues.inaccPutts -= ((int)upgradeSlider.value - oldSliderValue) / 100;
                break;
        }
        CharacterSelection.Instance.GetClickedButton().GetCharacterData().statValues = statValues;
    }

    private void UpdateStats()
    {
        Character.Stats stats = CharacterSelection.Instance.GetClickedButton().GetCharacterData().stats;
        switch (skill.text.Trim())
        {
            case "Luck":
                stats.luck = (int)upgradeSlider.value;
                break;
            case "Power":
                stats.power = (int)upgradeSlider.value;
                break;
            case "Accuracy":
                stats.accuracy = (int)upgradeSlider.value;
                break;
            case "Drives":
                stats.drives = (int)upgradeSlider.value;
                break;
            case "Winds":
                stats.winds = (int)upgradeSlider.value;
                break;
            case "Putts":
                stats.putts = (int)upgradeSlider.value;
                break;
        }
        CharacterSelection.Instance.GetClickedButton().GetCharacterData().stats = stats;
    }

    private async void PostSkills()
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

        gameData.SetCoins(tempCoins);

        string dataToSend = JsonConvert.SerializeObject(gameData);
        JObject response = await UserGameSkillPoster.PostGameSkill(dataToSend);

        if (!(bool)response["status"])
            ShowErrorMsg("Failed to update coins!");
        else
            DisablePopup();

        loadingBar.fillAmount = 0;
        loadingPanel.SetActive(false);
    }
    private async void ShowErrorMsg(string msg)
    {
        errorText.text = msg;
        errorText.gameObject.SetActive(true);
        await UniTask.Delay(2500);
        errorText.gameObject.SetActive(false);
    }
}
