using UnityEngine;

public class Clone_Skill_Controller : MonoBehaviour
{
    private SpriteRenderer sr;
    private Animator anim;

    [SerializeField] private float colorLosingSpeed;

    private float cloneTimer;
    private float attackMultiplier;
    [SerializeField] private Transform attackCheck;
    [SerializeField] private float attackCheckRadius = .8f;
    private Transform closestEnemy;
    private int facingDir = 1;

    private float chanceDulicate;
    private bool canDuplicateClone;
    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        cloneTimer -= Time.deltaTime;

        if(cloneTimer < 0)
        {
            sr.color = new Color(1, 1, 1, sr.color.a - (Time.deltaTime * colorLosingSpeed));
        }
        if(sr.color.a < 0)
        {
            Destroy(gameObject);
        }
    }

    public void SetupClone(Transform _newTransform, float _cloneDuration, bool _canAttack, Vector3 _offset, Transform _closestEnemy,bool _canDuplicateClone,float _chanceDulicate,float _attckMultiplier)
    {
        if(_canAttack)
            anim.SetInteger("AttackNumber", Random.Range(1,4));

        attackMultiplier = _attckMultiplier;
        transform.position = _newTransform.position + _offset;
        cloneTimer = _cloneDuration;

        closestEnemy = _closestEnemy;
        canDuplicateClone = _canDuplicateClone;
        chanceDulicate = _chanceDulicate;
        FaceClosestTarget();
    }

    private void AnimationTrigger()
    {
        cloneTimer = -.1f;
    }

    private void AttackTrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(attackCheck.position, attackCheckRadius);

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();
                playerStats.CloneDoDamage(hit.GetComponent<CharacterStats>(), attackMultiplier);

                if(PlayerManager.instance.player.skill.clone.canApplyHitEffect)
                {
                    ItemData_Equipment weaponData = Inventory.instance.GetEquipment(EquipmentType.Weapon);

                    if (weaponData != null)
                        weaponData.Effect(hit.transform);
                }

                if(canDuplicateClone)
                {
                    if(Random.Range(0,100) < chanceDulicate)
                    {
                        SkillManager.instance.clone.CreateClone(hit.transform, new Vector3(1.5f * facingDir, 0));
                    }
                }
            }
        }
    }

    private void FaceClosestTarget()
    {
        if(closestEnemy != null)
        {
            if(closestEnemy.position.x < transform.position.x)
            {
                facingDir = -1;
                transform.Rotate(0, 180, 0);
            }
        }
    }
}
