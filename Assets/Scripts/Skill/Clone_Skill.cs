using System.Collections;
using UnityEngine;

public class Clone_Skill : Skill
{
    [Header("Clone info")]
    [SerializeField] private GameObject clonePrefab;
    [SerializeField] private float cloneDuration;
    [Space]
    [SerializeField] private bool canAttack;

    [SerializeField] private bool canCloneDashStart;
    [SerializeField] private bool canCloneDashEnd;
    [SerializeField] private bool canCloneCounterAttack;
    [Header("clone duplicate")]
    [SerializeField] private bool canDuplicateClone;
    [SerializeField] private float chanceDuplicate;
    [Header("crystal instead of clone")]
    public bool crystalInsteadClone;
    
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
            SetupClone(_clonePosition, cloneDuration, canAttack, _offset, FindClosestEnemy(newClone.transform), canDuplicateClone, chanceDuplicate);
    }

    public void CloneDashStart()
    {
        if(canCloneDashStart)
        {
            CreateClone(player.transform, Vector3.zero);
        }
    }

    public void CloneDashEnd()
    {
        if (canCloneDashEnd)
        {
            CreateClone(player.transform, Vector3.zero);
        }
    }

    public void CloneCounterAttack(Transform _enemyTransform)
    {
        if (canCloneCounterAttack)
        {
            StartCoroutine(CloneDelay(_enemyTransform, new Vector3(2 * player.facingDir, 0)));
        }
    }

    private IEnumerator CloneDelay(Transform _enemyTransform,Vector3 _offset)
    {
        yield return new WaitForSeconds(.2f);
            CreateClone(_enemyTransform, _offset);
    }
}
