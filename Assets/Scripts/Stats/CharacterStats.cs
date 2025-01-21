using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public enum StatType
{
    strength,
    agility,
    intelligence,
    vitality,

    damage,
    cirtChance,
    cirtPower,

    maxHealth,
    armor,
    evasion,
    magicResistance,

    fireDamage,
    iceDamage,
    lightingDamage
}

public class CharacterStats : MonoBehaviour
{
    private EntityFX fx;

    [Header("Majior stats")]
    public Stat strength; // 每点提高1点伤害和1%暴击伤害
    public Stat agility; // 每点提高1点闪避和1%的暴击几率
    public Stat intelligence; // 每点提高1点魔法攻击和一点魔法抗性
    public Stat vitality; // 每点提高5点的生命值

    [Header("Offensive stats")]
    public Stat damage;
    public Stat cirtChance;
    public Stat cirtPower; // 基础值150

    [Header("Defencive stats")]
    public Stat maxHealth;
    public Stat armor;
    public Stat evasion;
    public Stat magicResistance;

    [Header("Magic stats")]
    public Stat fireDamage;
    public Stat iceDamage;
    public Stat lightingDamage;

    public bool isIgnited; // 随时间造成伤害
    public bool isChilled; // 降低护甲
    public bool isShocked; // 降低攻击准度20

    [SerializeField] private float ailmentsDuration = 4;
    private float ignitedTimer;
    private float chilledTimer;
    private float shockedTimer;

    private float ignitedDamageTimer;
    private float igniteDamageCooldown = .3f;
    private int igniteDamage;

    [SerializeField] private GameObject thunderStrikePrefabs;
    private int shockDamage;

    public int currentHealth;

    public System.Action onHealthChanged;

    public bool isDead {  get; private set; }
    public bool isVulnerable;

    protected virtual void Start()
    {
        cirtPower.SetDefaultValue(150);
        currentHealth = GetHealth();

        fx = GetComponent<EntityFX>();
    }

    protected virtual void Update()
    {
        if (isDead)
            return;

        ignitedTimer -= Time.deltaTime;
        chilledTimer -= Time.deltaTime;
        shockedTimer -= Time.deltaTime;
        ignitedDamageTimer -= Time.deltaTime;

        if (ignitedTimer < 0)
            isIgnited = false;
        if (chilledTimer < 0)
            isChilled = false;
        if (shockedTimer < 0)
            isShocked = false;
        
        if(isIgnited)
            ApplyIgniteDamage();
    }

    public void MakeVulnerableFor(float _duration) => StartCoroutine(VulnerableCorutine(_duration));

    private IEnumerator VulnerableCorutine(float _duration)
    {
        isVulnerable = true;

        yield return new WaitForSeconds(_duration);

        isVulnerable = false;
    }

    public int GetHealth()
    {
        return maxHealth.GetValue() + vitality.GetValue() * 5;
    }

    public virtual void IncreaseStat(int _modifier, float _duration,Stat _statToModify)
    {
        StartCoroutine(StatModCoroutine(_modifier, _duration, _statToModify));
    }

    IEnumerator StatModCoroutine(int _modifier, float _duration, Stat _statToModify)
    {
        _statToModify.AddModifier(_modifier);

        yield return new WaitForSeconds(_duration);

        _statToModify.RemoveModifier(_modifier);
    }

    public virtual void DoDamage(CharacterStats _targetStats)
    {
        if (CanAvoidAttack(_targetStats))
            return;

        int totalDamage = damage.GetValue() + strength.GetValue();

        if(CanCirt())
        {
            totalDamage = CirtDamage(totalDamage);
        }

        totalDamage = CheckTargetArmor(_targetStats, totalDamage);
        _targetStats.TakeDamage(totalDamage);

        DoMagicDamage(_targetStats);
    }

    #region 魔法伤害和魔法状态

    public virtual void DoMagicDamage(CharacterStats _targetStats)
    {
        int _fireDamage = fireDamage.GetValue();
        int _iceDamage = iceDamage.GetValue();
        int _lightingDamage = lightingDamage.GetValue();



        int totalMagicDamage = _fireDamage + _iceDamage + _lightingDamage + intelligence.GetValue();
        totalMagicDamage = CheckTargetResistance(_targetStats, totalMagicDamage);

        _targetStats.TakeDamage(totalMagicDamage);

        if (Mathf.Max(_fireDamage, _iceDamage, _lightingDamage) <= 0)
            return;
    
        AttemptToApplyAilements(_targetStats, _fireDamage, _iceDamage, _lightingDamage);
    }

    private void AttemptToApplyAilements(CharacterStats _targetStats, int _fireDamage, int _iceDamage, int _lightingDamage)
    {
        bool canApplyIgnite = _fireDamage > _iceDamage && _fireDamage > _lightingDamage;
        bool canApplyChill = _iceDamage > _fireDamage && _iceDamage > _lightingDamage;
        bool canApplyShock = _lightingDamage > _fireDamage && _lightingDamage > _iceDamage;

        while (!canApplyIgnite && !canApplyChill && !canApplyShock)
        {
            if (Random.value < .5f && _fireDamage > 0)
            {
                canApplyIgnite = true;
                Debug.Log("fire");
                _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                return;
            }
            if (Random.value < .5f && _iceDamage > 0)
            {
                canApplyChill = true;
                Debug.Log("ice");
                _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                return;
            }
            if (Random.value < .5f && _lightingDamage > 0)
            {
                canApplyShock = true;
                Debug.Log("light");
                _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                return;
            }
        }

        if (canApplyIgnite)
            _targetStats.SetupIgniteDamage(Mathf.RoundToInt(_fireDamage * .2f));

        if (canApplyShock)
            _targetStats.SetupThunderStrikeDamage(Mathf.RoundToInt(_lightingDamage * .1f));

        _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
    }

    public void ApplyAilments(bool _ignite,bool _chill,bool _shock)
    {
        bool canApplyIgnite = !isIgnited && !isChilled && !isShocked;
        bool canApplyChill = !isIgnited && !isChilled && !isShocked;
        bool canApplyShock = !isIgnited && !isChilled;

        if(_ignite && canApplyIgnite)
        {
            isIgnited = _ignite;
            ignitedTimer = ailmentsDuration;

            fx.IgniteFx(ailmentsDuration);
        }
        if(_chill && canApplyChill)
        {
            isChilled = _chill;
            chilledTimer = ailmentsDuration;

            float slowPrecentage = .2f; 
            GetComponent<Entity>().SlowEntity(slowPrecentage, ailmentsDuration);
            fx.ChillFx(ailmentsDuration);
        }
        if (_shock && canApplyShock)
        {
            if(!isShocked)
            {
                ApplyShock(_shock);
            }
            else
            {
                if (GetComponent<Player>() != null)
                    return;

                HitNearestTargetWithThunderStrike();
            }
        }
    }

    public void ApplyShock(bool _shock)
    {
        if (isShocked)
            return;

        isShocked = _shock;
        shockedTimer = ailmentsDuration;

        fx.ShockFx(ailmentsDuration);
    }

    private void HitNearestTargetWithThunderStrike()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 25);

        float closestDistance = Mathf.Infinity;

        Transform closestEnemy = null;
        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null && Vector2.Distance(transform.position, hit.transform.position) > 1)
            {
                float distanceToEnemy = Vector2.Distance(transform.position, hit.transform.position);

                if (distanceToEnemy < closestDistance)
                {
                    closestDistance = distanceToEnemy;
                    closestEnemy = hit.transform;
                }

                if (closestEnemy == null)
                    closestEnemy = transform;
            }
        }

        if (closestEnemy != null)
        {
            GameObject newThunderStrike = Instantiate(thunderStrikePrefabs, transform.position, Quaternion.identity);

            newThunderStrike.GetComponent<ThunderStrike_Controller>().Setup(shockDamage, closestEnemy.GetComponent<CharacterStats>());
        }
    }

    private void ApplyIgniteDamage()
    {
        if (ignitedDamageTimer < 0)
        {
            DecreaseHealth(igniteDamage);

            if (currentHealth < 0)
            {
                Die();
            }

            ignitedDamageTimer = igniteDamageCooldown;
        }
    }

    public void SetupIgniteDamage(int _damage) => igniteDamage = _damage;

    public void SetupThunderStrikeDamage(int _damage) => shockDamage = _damage;

    #endregion

    public virtual void TakeDamage(int _damage)
    {
        if (isDead)
            return;
        DecreaseHealth(_damage);

        GetComponent<Entity>().DamageImpact();
        fx.StartCoroutine("FlashFX");

        if (currentHealth <= 0)
        {
            Die();
            Debug.Log("take damage");
        }
    }

    public virtual void IncreaseHealth(int _amount)
    {
        currentHealth += _amount;

        if(currentHealth > GetHealth())
            currentHealth = GetHealth();

        if (onHealthChanged != null)
            onHealthChanged();
    }

    protected virtual void DecreaseHealth(int _damage)
    {
        if (isVulnerable)
            _damage = Mathf.RoundToInt(_damage * 1.5f);

        currentHealth -= _damage;

        if (onHealthChanged != null)
            onHealthChanged();
    }

    protected virtual void Die()
    {
        if (isDead)
            return;

        isDead = true;
    }

    #region 统计计算
    protected int CheckTargetArmor(CharacterStats _targetStats, int totalDamage)
    {
        if (_targetStats.isChilled)
            totalDamage -= Mathf.RoundToInt(_targetStats.armor.GetValue() * .8f);
        else
            totalDamage -= _targetStats.armor.GetValue();

        totalDamage = Mathf.Clamp(totalDamage, 0, int.MaxValue);
        return totalDamage;
    }

    private int CheckTargetResistance(CharacterStats _targetStats, int totalMagicDamage)
    {
        totalMagicDamage -= _targetStats.magicResistance.GetValue() + _targetStats.intelligence.GetValue() * 3;
        totalMagicDamage = Mathf.Clamp(totalMagicDamage, 0, int.MaxValue);
        return totalMagicDamage;
    }

    public virtual void OnEvasion()
    {
    }

    protected bool CanAvoidAttack(CharacterStats _targetStats)
    {
        int totalEvasion = _targetStats.evasion.GetValue() + _targetStats.agility.GetValue();

        if (isShocked)
            totalEvasion += 20;

        if (Random.Range(0, 100) < totalEvasion)
        {
            _targetStats.OnEvasion();
            return true;
        }
        return false;
    }

    protected bool CanCirt()
    {
        int totalCirtChance = cirtChance.GetValue() + agility.GetValue();

        if(Random.Range(0, 100) < totalCirtChance)
        {
            return true;
        }
        return false;
    }

    protected int CirtDamage(int _damage)
    {
        float totalCirtPower = (cirtPower.GetValue() + strength.GetValue()) * .01f;
        float cirtDamage = _damage * totalCirtPower;

        return Mathf.RoundToInt(cirtDamage);
    }

    #endregion

    public Stat GetStat(StatType _statType)
    {
        switch (_statType)
        {
            case StatType.strength: return strength;
            case StatType.agility: return agility;
            case StatType.intelligence: return intelligence;
            case StatType.vitality: return vitality;

            case StatType.damage: return damage;
            case StatType.cirtChance: return cirtChance;
            case StatType.cirtPower: return cirtPower;

            case StatType.maxHealth: return maxHealth;
            case StatType.armor: return armor;
            case StatType.evasion: return evasion;
            case StatType.magicResistance: return magicResistance;

            case StatType.fireDamage: return fireDamage;
            case StatType.iceDamage: return iceDamage;
            case StatType.lightingDamage: return lightingDamage;
        }
        return null;
    }
}
