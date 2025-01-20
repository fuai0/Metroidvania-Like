using TMPro;
using UnityEngine;

public class UI_SkillToolTip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI skillName;
    [SerializeField] private TextMeshProUGUI skillDescription;
    [SerializeField] private TextMeshProUGUI skillCost;

    public void ShowToolTip(string _skillName, string _skillDescription, int _price)
    {
        skillName.text = _skillName;
        skillDescription.text = _skillDescription;
        skillCost.text = "Cost : " + _price.ToString();
        gameObject.SetActive(true);
    }

    public void HideToolTip() => gameObject.SetActive(false);
}
