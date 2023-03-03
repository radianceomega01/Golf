using DanielLochner.Assets.SimpleScrollSnap;
using UnityEngine;
using UnityEngine.UI;

public class CharacterButton : MonoBehaviour
{
    [SerializeField] private Character characterData;
    [SerializeField] private GameObject character;

    private SimpleScrollSnap scrollSnap;
    private RectTransform rectTransform;
    private Button button;

    private void Awake()
    {
        scrollSnap = GetComponentInParent<SimpleScrollSnap>();
        scrollSnap.OnPanelCentered.AddListener(MakePanelChanges);
        rectTransform = GetComponentInChildren<Image>().GetComponent<RectTransform>();
        button = GetComponent<Button>();
        button.onClick.AddListener(SwitchPanel);

    }
    private void Start()
    {
        MakePanelChanges(0,0);
    }

    private void MakePanelChanges(int xIndex, int yIndex)
    {
        if (transform.GetSiblingIndex() != xIndex)
        {
            rectTransform.localScale = Vector3.one;
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            return;
        }

        rectTransform.localScale = Vector3.one * 1.25f;
        rectTransform.pivot = new Vector2(0.5f, 0f);
        CharacterSelection.Instance.SetCharacter(character, gameObject);
        CharacterSelection.Instance.SetDescription(characterData);
        CharacterSelection.Instance.SetSkills(characterData);
    }

    private void SwitchPanel()
    {
        scrollSnap.GoToPanel(transform.GetSiblingIndex());
    }
    public Character GetCharacterData() => characterData;
}
