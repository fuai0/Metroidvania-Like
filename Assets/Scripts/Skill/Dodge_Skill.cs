using UnityEngine;
using UnityEngine.UI;

public class Dodge_Skill : Skill
{
    [Header("dodge")]
    [SerializeField] private UI_SkillTreeSlot unlockDodgeButton;
    [SerializeField] private int evasionAmount;
    public bool dodgeUnlocked {  get; private set; }

    [Header("mirage dodge")]
    [SerializeField] private UI_SkillTreeSlot unlockMirageDodgeButton;
    public bool mirageDodgeUnlocked { get; private set; }

    protected override void Start()
    {
        base.Start();

        unlockDodgeButton.GetComponent<Button>().onClick.AddListener(UnlockDodge);
        unlockMirageDodgeButton.GetComponent<Button>().onClick.AddListener(UnlockDodge);
    }

    #region ½âËø¼¼ÄÜ

    protected override void CheckUnlock()
    {
        UnlockDodge();
        UnlockMirageDodge();
    }

    private void UnlockDodge()
    {
        if(unlockDodgeButton.unlocked && !dodgeUnlocked)
        {
            player.stats.evasion.AddModifier(evasionAmount);
            Inventory.instance.UpdateStatsUI();
            dodgeUnlocked = true;
        }
    }

    private void UnlockMirageDodge()
    {
        if(unlockMirageDodgeButton.unlocked)
            mirageDodgeUnlocked = true;
    }

    #endregion

    public void CreateMirageOnDodge()
    {
        if (mirageDodgeUnlocked)
            SkillManager.instance.clone.CreateClone(player.transform, new Vector2(2 * player.facingDir, 0));
    }

}