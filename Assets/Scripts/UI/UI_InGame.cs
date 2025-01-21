using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UI_InGame : MonoBehaviour
{
    private PlayerStats playerStats;
    [SerializeField] private Slider slider;

    [SerializeField] private Image dashImage;
    [SerializeField] private Image parryImage;
    [SerializeField] private Image crystalImage;
    [SerializeField] private Image swordImage;
    [SerializeField] private Image blackholeImage;
    [SerializeField] private Image flaskImage;

    private SkillManager skills;

    [Header("Souls info")]
    [SerializeField] private TextMeshProUGUI currentSouls;
    [SerializeField] private float soulsAmount;
    [SerializeField] private float increaseRate = 100;

    private void Start()
    {
        playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        if (playerStats != null)
            playerStats.onHealthChanged += UpdateHealthUI;

        skills = SkillManager.instance;

        UpdateHealthUI();
    }

    private void Update()
    {
        UpdateSoulsUI();

        if (Input.GetKeyDown(KeyCode.LeftShift) && skills.dash.dashUnlocked)
            SetCooldown(dashImage);

        if (Input.GetKeyDown(KeyCode.Q) && skills.parry.parryUnlocked)
            SetCooldown(parryImage);

        if (Input.GetKeyDown(KeyCode.F) && skills.crystal.crystalUnlocked)
            SetCooldown(crystalImage);

        if (Input.GetKeyDown(KeyCode.Mouse1) && skills.sword.swordUnlocked)
            SetCooldown(swordImage);

        if (Input.GetKeyDown(KeyCode.R) && skills.blackHole.blackholeUnlocked)
            SetCooldown(blackholeImage);

        if (Input.GetKeyDown(KeyCode.Alpha1) && Inventory.instance.GetEquipment(EquipmentType.Flask) != null)
            SetCooldown(flaskImage);

        CheckCoolDown(dashImage, skills.dash.cooldown);
        CheckCoolDown(parryImage, skills.parry.cooldown);
        CheckCoolDown(crystalImage, skills.crystal.cooldown);
        CheckCoolDown(swordImage, skills.sword.cooldown);
        CheckCoolDown(blackholeImage, skills.blackHole.cooldown);
        CheckCoolDown(flaskImage, Inventory.instance.flaskCooldown);
    }

    private void UpdateSoulsUI()
    {
        if (soulsAmount < PlayerManager.instance.GetCurrency())
            soulsAmount += Time.deltaTime * increaseRate;
        else
            soulsAmount = PlayerManager.instance.GetCurrency();

        currentSouls.text = ((int)soulsAmount).ToString("#,#");
    }

    private void UpdateHealthUI()
    {
        slider.maxValue = playerStats.GetHealth();
        slider.value = playerStats.currentHealth;
    }

    private void SetCooldown(Image _image)
    {
        if (_image.fillAmount <= 0)
            _image.fillAmount = 1;
    }

    private void CheckCoolDown(Image _image, float _cooldown)
    {
        if(_image.fillAmount > 0)
            _image.fillAmount -= 1/_cooldown*Time.deltaTime;
    }
}
