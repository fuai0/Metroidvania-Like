using UnityEngine;


[CreateAssetMenu(fileName = "Heal Effext", menuName = "Data/Item Effect/Heal Effext")]

public class Heal_Effect : ItemEffect
{
    [Range(0f, 1f)]
    [SerializeField] private float healPrecent;

    public override void ExecuteEffect(Transform _enemyPosition)
    {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        int healAmount = Mathf.RoundToInt(playerStats.maxHealth.GetValue() * healPrecent);

        playerStats.IncreaseHealth(healAmount);
    }
}
