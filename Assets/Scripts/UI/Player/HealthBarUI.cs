using UnityEngine;

public class HealthBarUI : MonoBehaviour
{
    public float maxHealth;
    public float currentHealth;
    public float width;
    public float height;

    [SerializeField]
    private RectTransform healthBar;

    public void SetMaxHealth(float maxHp)
    {
        maxHealth = maxHp;
        currentHealth = maxHealth;
    }

    public void SetHealth(float healthChange)
    {
        currentHealth -= healthChange;
        float newWidth = (currentHealth / maxHealth) * width;

        healthBar.sizeDelta = new Vector2(newWidth, height);
    }
}
