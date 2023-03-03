
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour
{
    [SerializeField] private Slider upgradeSlider;
    [SerializeField] private Slider skillSlider;
    [SerializeField] private TextMeshProUGUI skillText;

    private Button button;
    private SkillUpgrader skillUpgrader;
    void Awake()
    {
        skillUpgrader = SkillUpgrader.Instance;
        button = GetComponent<Button>();
        button.onClick.AddListener(UpdateValue);
        upgradeSlider.value = skillSlider.value;
        upgradeSlider.minValue = skillSlider.value;
        upgradeSlider.maxValue = skillSlider.maxValue;
    }

    private void UpdateValue()
    {
        skillUpgrader.EnablePopup();
        skillUpgrader.SetValues(upgradeSlider, skillText.text);
    }
}
