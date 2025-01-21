using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

public class Crystal_Skill : Skill
{
    [SerializeField] private float crystalDuration;
    [SerializeField] private GameObject crystalPrefab;
    private GameObject currentCrystal;

    [Header("crystal simple")]
    [SerializeField] private UI_SkillTreeSlot unlockCrystalButton;
    public bool crystalUnlocked { get; private set; }

    [Header("crystal mirage")]
    [SerializeField] private UI_SkillTreeSlot unlockCloneInsteadButton;
    [SerializeField] private bool cloneInsteadOfCrystal;


    [Header("explosive crystal")]
    [SerializeField] private UI_SkillTreeSlot unlockExplosiveButton;
    [SerializeField] private bool canExplode;

    [Header("moving crystal")]
    [SerializeField] private UI_SkillTreeSlot unlockMovingButton;
    [SerializeField] private bool canMove;
    [SerializeField] private float moveSpeed;

    [Header("mult stacking crystal")]
    [SerializeField] private UI_SkillTreeSlot unlockMultStackButton;
    [SerializeField] private bool canUseMultStacks;
    [SerializeField] private float amountOfStacks;
    [SerializeField] private float multStackCooldown;
    [SerializeField] private float useTimeWindow;
    [SerializeField] private List<GameObject> crystalLeft = new List<GameObject>();

    protected override void Start()
    {
        base.Start();

        unlockCrystalButton.GetComponent<Button>().onClick.AddListener(UnlockCrystal);
        unlockCloneInsteadButton.GetComponent<Button>().onClick.AddListener(UnlockCrystalMirage);
        unlockExplosiveButton.GetComponent<Button>().onClick.AddListener(UnlockExplosiveCrystal);
        unlockMovingButton.GetComponent<Button>().onClick.AddListener(UnlockMovingCrystal);
        unlockMultStackButton.GetComponent<Button>().onClick.AddListener(UnlockMultStack);
    }


    #region ½âËø¼¼ÄÜ

    protected override void CheckUnlock()
    {
        UnlockCrystal();
        UnlockCrystalMirage();
        UnlockExplosiveCrystal();
        UnlockMovingCrystal();
        UnlockMultStack();
    }

    private void UnlockCrystal()
    {
        if (unlockCrystalButton.unlocked)
            crystalUnlocked = true;
    }

    private void UnlockCrystalMirage()
    {
        if (unlockCloneInsteadButton.unlocked)
            cloneInsteadOfCrystal = true;
    }

    private void UnlockExplosiveCrystal()
    {
        if (unlockExplosiveButton.unlocked)
            canExplode = true;
    }

    private void UnlockMovingCrystal()
    {
        if (unlockMovingButton.unlocked)
            canMove = true;
    }

    private void UnlockMultStack()
    {
        if (unlockMultStackButton.unlocked)
            canUseMultStacks = true;
    }

    #endregion

    public override bool CanUseSkill()
    {
        return base.CanUseSkill();
    }

    public override void UseSkill()
    {
        base.UseSkill();

        if(CanUseMultCrystal())
        {
            return;
        }

        if(currentCrystal == null)
        {
            CreateCrystal();
        }
        else
        {
            if (canMove)
                return;

            Vector2 playerPos = player.transform.position;
            player.transform.position = currentCrystal.transform.position;
            currentCrystal.transform.position = playerPos;

            if(cloneInsteadOfCrystal)
            {
                SkillManager.instance.clone.CreateClone(currentCrystal.transform, Vector3.zero);
                Destroy(currentCrystal);
            }
            else
            {
                currentCrystal.GetComponent<Crystal_Skill_Controller>()?.FinishCrystal();
            }
        }
    }

    public void CreateCrystal()
    {
        currentCrystal = Instantiate(crystalPrefab, player.transform.position, Quaternion.identity);
        Crystal_Skill_Controller currentCrystalScript = currentCrystal.GetComponent<Crystal_Skill_Controller>();

        currentCrystalScript.SetupCrystal(crystalDuration, canExplode, canMove, moveSpeed, FindClosestEnemy(currentCrystal.transform));
    }

    public void CurrentCrystalChooseRandomTarget() => currentCrystal.GetComponent<Crystal_Skill_Controller>().ChooseRandomEnemy();

    private bool CanUseMultCrystal()
    {
        if(canUseMultStacks)
        {
            if(crystalLeft.Count > 0)
            {
                if (crystalLeft.Count == amountOfStacks)
                    Invoke("ResetAbility", useTimeWindow);
                cooldown = 0;
                GameObject crystalToSpawn = crystalLeft[crystalLeft.Count - 1];
                GameObject newCrystal = Instantiate(crystalToSpawn, player.transform.position, Quaternion.identity);

                crystalLeft.Remove(crystalToSpawn);
                newCrystal.GetComponent<Crystal_Skill_Controller>().
                    SetupCrystal(crystalDuration, canExplode, canMove, moveSpeed, FindClosestEnemy(newCrystal.transform));

                if(crystalLeft.Count <= 0)
                {
                    cooldown = multStackCooldown;
                    RefilCrystal();
                }

            return true;
            }

        }

        return false;
    }

    private void RefilCrystal()
    {
        float amountAdd = amountOfStacks - crystalLeft.Count;
        for(int i = 0;i < amountAdd;i++)
        {
            crystalLeft.Add(crystalPrefab);
        }
    }

    private void ResetAbility()
    {
        if (cooldownTimer > 0)
            return;

        cooldownTimer = multStackCooldown;
        RefilCrystal();
    }
}
