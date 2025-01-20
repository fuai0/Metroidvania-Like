using UnityEngine;
using UnityEngine.UI;

public class UI_HeathBar : MonoBehaviour
{
    private Entity entity;
    private CharacterStats stats;
    private RectTransform myTransform;
    [SerializeField] private Image image; 

    private void Start()
    {
        myTransform = GetComponent<RectTransform>();
        entity = GetComponentInParent<Entity>();
        stats = GetComponentInParent<CharacterStats>();
        
        entity.onFlipped += FlipUI;
        stats.onHealthChanged += UpdateHealthUI;

        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        float health = stats.currentHealth;
        image.fillAmount = health / stats.GetHealth();
    }


    private void FlipUI() => myTransform.Rotate(0, 180, 0);
    private void OnDisable()
    {
        entity.onFlipped -= FlipUI;
        stats.onHealthChanged -= UpdateHealthUI;
    }
}
