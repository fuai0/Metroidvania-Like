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

[CreateAssetMenu(fileName = "Buff Effect", menuName = "Data/Item Effect/Buff Effect")]
public class Buff_Effect : ItemEffect
{
    private PlayerStats stats;
    [SerializeField] private StatType buffType;
    [SerializeField] private int buffAmount;
    [SerializeField] private float buffDuration;


    public override void ExecuteEffect(Transform _enemyPosition)
    {
        stats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        stats.IncreaseStat(buffAmount, buffDuration, StatToModify());
    }

    private Stat StatToModify()
    {
        switch (buffType)
        {
            case StatType.strength:         return stats.strength;
            case StatType.agility:          return stats.agility;
            case StatType.intelligence:     return stats.intelligence;
            case StatType.vitality:         return stats.vitality;

            case StatType.damage:           return stats.damage;
            case StatType.cirtChance:       return stats.cirtChance;
            case StatType.cirtPower:        return stats.cirtPower;

            case StatType.maxHealth:        return stats.maxHealth;
            case StatType.armor:            return stats.armor;
            case StatType.evasion:          return stats.evasion;
            case StatType.magicResistance:  return stats.magicResistance;

            case StatType.fireDamage:       return stats.fireDamage;
            case StatType.iceDamage:        return stats.iceDamage;
            case StatType.lightingDamage:   return stats.lightingDamage;
        }
        return null;
    }
}
