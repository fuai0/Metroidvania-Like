using UnityEngine;
using UnityEngine.UI;

public class Parry_Skill : Skill
{
    [Header("parry")]
    [SerializeField] private UI_SkillTreeSlot parryUnlockButton;
    public bool parryUnlocked {  get; private set; }

    [Header("parry restore")]
    [SerializeField] private UI_SkillTreeSlot restoreUnlockButton;
    [Range(0,1f)]
    [SerializeField] private float restoreHealthPrecent;
    public bool restoreUnlocked {  get; private set; }

    [Header("parry with mirage")]
    [SerializeField] private UI_SkillTreeSlot parryWithMirageUnlockButton;
    public bool parryWithMirageUnlocked { get; private set; }

    public override void UseSkill()
    {
        base.UseSkill();

        if (restoreUnlocked)
        {
            int restoreAmount = Mathf.RoundToInt(player.stats.GetHealth() * restoreHealthPrecent);
            player.stats.IncreaseHealth(restoreAmount);
        }
    }

    protected override void Start()
    {
        base.Start();

        parryUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockParry);
        restoreUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockRestore);
        parryWithMirageUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockParryWithMirage);
    }

    #region ½âËø¼¼ÄÜ

    protected override void CheckUnlock()
    {
        UnlockParry();
        UnlockRestore();
        UnlockParryWithMirage();
    }

    private void UnlockParry()
    {
        if (parryUnlockButton.unlocked)
            parryUnlocked = true;
    }

    private void UnlockRestore()
    {
        if (restoreUnlockButton.unlocked)
            restoreUnlocked = true;
    }

    private void UnlockParryWithMirage()
    {
        if (parryWithMirageUnlockButton.unlocked)
            parryWithMirageUnlocked = true;
    }

    #endregion

    public void MakeMirageOnParry(Transform _respawnTransform)
    {
        if (parryWithMirageUnlocked)
            SkillManager.instance.clone.CreateCloneWithDelay(_respawnTransform);
    }
}
