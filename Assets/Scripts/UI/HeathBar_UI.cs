using UnityEngine;
using UnityEngine.UI;

public class HeathBar_UI : MonoBehaviour
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
        float currentHealth = stats.currentHealth;
        image.fillAmount = currentHealth / (stats.maxHealth.GetValue() + stats.vitality.GetValue());
    }


    private void FlipUI() => myTransform.Rotate(0, 180, 0);
    private void OnDisable()
    {
        entity.onFlipped -= FlipUI;
        stats.onHealthChanged -= UpdateHealthUI;
    }
}
