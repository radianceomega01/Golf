using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ClubOption : MonoBehaviour
{
    [SerializeField] private float selectionSizeGain = 1.2f;

    private Club clubData;
    private Button button;
    //private TextMeshProUGUI text;
    private LayoutElement layoutElement;
    private AspectRatioFitter aspectRatioFitter;
    private ClubSelectionSystem clubSelectionSystem;

    private void Awake()
    {
        button = GetComponent<Button>();
        //text = GetComponentInChildren<TextMeshProUGUI>();
        layoutElement = GetComponent<LayoutElement>();
        aspectRatioFitter = GetComponent<AspectRatioFitter>();
        clubSelectionSystem = GetComponentInParent<ClubSelectionSystem>();

        layoutElement.flexibleHeight = 1;
        aspectRatioFitter.aspectMode = AspectRatioFitter.AspectMode.None;

        button.onClick.AddListener(OnButtonClick);
        //clubSelectionSystem.OnClubChanged += OnClubChanged;

        button.image.sprite = clubData.clubIcon;
    }

    private IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        var rectTransform = ((RectTransform)transform);
        var size = rectTransform.rect.size;
        aspectRatioFitter.aspectRatio = size.x / size.y;
        rectTransform.pivot = new Vector2(1, 0.5f);
    }

    public void SetClub(Club value)
    {
        clubData = value;
        //text.text = clubData.name;
    }

    private void Select()
    {
        Golfer golfer = MatchManager.Instance.GetActiveGolfer();
        var clubObject = golfer.ClubHolder.Find(clubData.name);
        if (clubObject == null)
            Instantiate(clubData.prefab, golfer.ClubHolder).name = clubData.name;
        else
            clubObject.gameObject.SetActive(true);

        layoutElement.flexibleHeight = selectionSizeGain;
        aspectRatioFitter.aspectMode = AspectRatioFitter.AspectMode.HeightControlsWidth;
    }

    private void Deselect()
    {
        Golfer golfer = MatchManager.Instance.GetActiveGolfer();
        var clubObject = golfer.ClubHolder.Find(clubData.name);
        if (clubObject != null)
            clubObject.gameObject.SetActive(false);

        aspectRatioFitter.aspectMode = AspectRatioFitter.AspectMode.None;
        layoutElement.flexibleHeight = 1;
    }

    private void OnButtonClick()
    {
        clubSelectionSystem.SelectClub(clubData);
        clubSelectionSystem.ToggleParentLayout(false);
    }

    private void OnClubChanged(Club club)
    {
        if (club == this.clubData)
            Select();
        else
            Deselect();
    }
}