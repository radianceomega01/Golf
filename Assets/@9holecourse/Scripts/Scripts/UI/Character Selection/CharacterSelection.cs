using Blueberry.Core.SceneManagement;
using Photon.Pun;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterSelection : SingletonBehaviour<CharacterSelection>
{
    [SerializeField] private new TextMeshProUGUI name;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private GameObject skills;
    [SerializeField] private GameObject charctersList;
    [SerializeField] private SelectionType selectionType;
    [SerializeField] private Button confirm;
    [SerializeField] private GameObject timerParent;
    [SerializeField] private Timer timer;

    private GameObject activeCharacter;
    private GameObject clickedButton;
    private float timerTime = 18;

    public event Action OnSkillsSet;
    //private CharacterList characterList;
    //private int selectedAvatarId;

    protected override void Awake()
    {
        if(confirm != null)
            confirm.onClick.AddListener(OnConfirmClicked);
        if(timer != null)
            timer.OnTimerComplete += LoadNextScene;
        //timerTime += Random.Range(0f, 2f);
    }

    void Start()
    {
        //characterList = CharacterList.Instance;
        //charctersList.transform.GetChild(0).GetComponent<CharacterButton>().OnButtonClick();
        if (GameManager.Instance.GameMode != GameManager.GameModes.SinglePlayer)
        {
            if (timerParent == null)
                return;
            timerParent.SetActive(true);
            timer.StartTimer(timerTime);
        }
            
    }

    public void SetCharacter(GameObject character, GameObject clickedButton)
    {
        this.clickedButton = clickedButton;

        if (activeCharacter != null)
            activeCharacter.SetActive(false);
        character.SetActive(character.name == clickedButton.name);

        activeCharacter = character;

    }
    public void SetDescription(Character characterData)
    {
        name.text = characterData.name;
        description.text = characterData.description;
        //selectedAvatarId = characterData.avatarId;
    }

    public void SetSkills(Character characterData)
    {
        skills.transform.GetChild(0).GetChild(1).GetComponent<Slider>().value = characterData.stats.luck;
        skills.transform.GetChild(1).GetChild(1).GetComponent<Slider>().value = characterData.stats.power;
        skills.transform.GetChild(2).GetChild(1).GetComponent<Slider>().value = characterData.stats.accuracy;
        skills.transform.GetChild(3).GetChild(1).GetComponent<Slider>().value = characterData.stats.drives;
        skills.transform.GetChild(4).GetChild(1).GetComponent<Slider>().value = characterData.stats.putts;
        skills.transform.GetChild(5).GetChild(1).GetComponent<Slider>().value = characterData.stats.winds;

        if (SceneManager.GetActiveScene().name.Equals("UpgradeSkills"))
            SetUpgradeSlider(characterData);
    }

    private void SetUpgradeSlider(Character characterData)
    {
        skills.transform.GetChild(0).GetChild(0).GetComponent<Slider>().value = characterData.stats.luck;
        skills.transform.GetChild(1).GetChild(0).GetComponent<Slider>().value = characterData.stats.power;
        skills.transform.GetChild(2).GetChild(0).GetComponent<Slider>().value = characterData.stats.accuracy;
        skills.transform.GetChild(3).GetChild(0).GetComponent<Slider>().value = characterData.stats.drives;
        skills.transform.GetChild(4).GetChild(0).GetComponent<Slider>().value = characterData.stats.putts;
        skills.transform.GetChild(5).GetChild(0).GetComponent<Slider>().value = characterData.stats.winds;
    }
    public void OnConfirmClicked()
    {
        //confirm.interactable = false;
        confirm.gameObject.SetActive(false);

        if (selectionType == SelectionType.PlayerSelection)
            MatchDetails.character = clickedButton.GetComponent<CharacterButton>().GetCharacterData();
        else
            MatchDetails.opponent = clickedButton.GetComponent<CharacterButton>().GetCharacterData();

        if (GameManager.Instance.GameMode == GameManager.GameModes.SinglePlayer)
        {
            if (selectionType == SelectionType.PlayerSelection)
                SceneHandler.Load("Opponent Selection");
            else
                SceneHandler.Load("Main");
        }
    }
    public CharacterButton GetClickedButton() => clickedButton.GetComponent<CharacterButton>();
    private void LoadNextScene()
    {
        //SceneHandler.Load("Main");
        if (MatchDetails.character == null)
            MatchDetails.character = CharacterList.Instance.GetCharacter(0);

        PhotonNetwork.LoadLevel("Main");
    }

    private void OnDestroy()
    {
        timer.OnTimerComplete -= LoadNextScene;
    }
    private enum SelectionType
    { 
        PlayerSelection,
        OpponentSelection
    }
}
