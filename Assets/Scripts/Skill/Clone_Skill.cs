using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Clone_Skill : Skill
{
    [Header("clone info")]
    [SerializeField] private float attackMultiplier;
    [SerializeField] private GameObject clonePrefab;
    [SerializeField] private float cloneDuration;

    [Header("clone attack")]
    [SerializeField] private UI_SkillTreeSlot cloneAttackButton;
    [SerializeField] private float cloneAttackMultiplier; 
    [SerializeField] private bool canAttack;

    [Header("aggresive clone")]
    [SerializeField] private UI_SkillTreeSlot aggresiveCloneButton;
    [SerializeField] private float aggresiveAttackMultiplier;
    public bool canApplyHitEffect;

    [Header("multiple clone")]
    [SerializeField] private UI_SkillTreeSlot multCloneButton;
    [SerializeField] private float multAttackMultiplier;
    [SerializeField] private bool canDuplicateClone;
    [SerializeField] private float chanceDuplicate;

    [Header("crystal instead of clone")]
    [SerializeField] private UI_SkillTreeSlot crystalCloneButton;
    public bool crystalInsteadClone;

    protected override void Start()
    {
        base.Start();

        cloneAttackButton.GetComponent<Button>().onClick.AddListener(UnlockCloneAttack);
        aggresiveCloneButton.GetComponent<Button>().onClick.AddListener(UnlockAggresiveAttack);
        multCloneButton.GetComponent<Button>().onClick.AddListener(UnlockMultClone);
        crystalCloneButton.GetComponent<Button>().onClick.AddListener(UnlockCrystalClone);
    }


    #region ½âËø¼¼ÄÜ

    protected override void CheckUnlock()
    {
        UnlockCloneAttack();
        UnlockAggresiveAttack();
        UnlockMultClone();
        UnlockCrystalClone();
    }

    private void UnlockCloneAttack()
    {
        if (cloneAttackButton.unlocked)
        {
            attackMultiplier = cloneAttackMultiplier;        
            canAttack = true;
        }
    }

    private void UnlockAggresiveAttack()
    {
        if (aggresiveCloneButton.unlocked)
        {
            attackMultiplier = aggresiveAttackMultiplier;
            canApplyHitEffect = true;
        }
    }

    private void UnlockMultClone()
    {
        if (multCloneButton.unlocked)
        {
            attackMultiplier = multAttackMultiplier;
            canDuplicateClone = true;
        }
    }

    private void UnlockCrystalClone()
    {
        if (crystalCloneButton.unlocked)
            crystalInsteadClone = true;
    }

    #endregion

    public void CreateClone(Transform _clonePosition,Vector3 _offset)
    {
        if(crystalInsteadClone)
        {
            SkillManager.instance.crystal.CreateCrystal();
            SkillManager.instance.crystal.CurrentCrystalChooseRandomTarget();
            return;
        }

        GameObject newClone = Instantiate(clonePrefab);
        newClone.GetComponent<Clone_Skill_Controller>().
            SetupClone(_clonePosition, cloneDuration, canAttack, _offset, FindClosestEnemy(newClone.transform), canDuplicateClone, chanceDuplicate, attackMultiplier);
    }

    public void CreateCloneWithDelay(Transform _enemyTransform)
    {
        StartCoroutine(CloneDelayCorotine(_enemyTransform, new Vector3(2 * player.facingDir, 0)));
    }

    private IEnumerator CloneDelayCorotine(Transform _enemyTransform,Vector3 _offset)
    {
        yield return new WaitForSeconds(.2f);
            CreateClone(_enemyTransform, _offset);
    }
}
