using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_StatSlot : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    private UI ui;

    [SerializeField] private string statName;
    [SerializeField] private StatType statType;
    [SerializeField] private TextMeshProUGUI statNameText;
    [SerializeField] private TextMeshProUGUI statValueText;

    [TextArea]
    [SerializeField] private string statDescription;

    private void OnValidate()
    {
        gameObject.name = "Stat - " + statName;

        if(statNameText != null )
            statNameText.text = statName;
    }

    private void Start()
    {
        UpdateStatValue();

        ui = GetComponentInParent<UI>();
    }

    public void UpdateStatValue()
    {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        if( playerStats != null )
        {
            statValueText.text = playerStats.GetStat(statType).GetValue().ToString();
        }


        if (statType == StatType.maxHealth)
            statValueText.text = playerStats.GetHealth().ToString();
        if(statType == StatType.damage)
            statValueText.text = (playerStats.strength.GetValue() + playerStats.damage.GetValue()).ToString();
        if (statType == StatType.cirtPower)
            statValueText.text = (playerStats.cirtPower.GetValue() + playerStats.strength.GetValue()).ToString();
        if (statType == StatType.cirtChance)
            statValueText.text = (playerStats.cirtChance.GetValue() + playerStats.agility.GetValue()).ToString();
        if (statType == StatType.evasion)
            statValueText.text = (playerStats.evasion.GetValue() + playerStats.agility.GetValue()).ToString();
        if (statType == StatType.magicResistance)
            statValueText.text = (playerStats.magicResistance.GetValue() + playerStats.intelligence.GetValue()).ToString();
        if (statType == StatType.fireDamage)
            statValueText.text = (playerStats.fireDamage.GetValue() + playerStats.intelligence.GetValue()).ToString();
        if (statType == StatType.iceDamage)
            statValueText.text = (playerStats.iceDamage.GetValue() + playerStats.intelligence.GetValue()).ToString();
        if (statType == StatType.lightingDamage)
            statValueText.text = (playerStats.lightingDamage.GetValue() + playerStats.intelligence.GetValue()).ToString();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ui.statToolTip.ShowToolTip(statDescription);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ui.statToolTip.HideToolTip();
    }
}
