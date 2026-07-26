using UnityEngine;
using UnityEngine.UI;

public class BaseHealthBar : MonoBehaviour
{
    [SerializeField] private Image fill;

    public void UpdateHealthBar(float healthPercent)
    {
        fill.fillAmount = Mathf.Clamp01(healthPercent);
        if (fill.fillAmount <= 0.2f)
        {
            fill.color = Color.red;
        }
        else if (fill.fillAmount <= 0.5f)
        {
            fill.color = Color.yellow;
        }
        else
        {
            fill.color = Color.green;
        }
    }
}
