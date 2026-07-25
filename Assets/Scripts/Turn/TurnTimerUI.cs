using UnityEngine;
using UnityEngine.UI;

public class TurnTimerUI : MonoBehaviour
{
    [SerializeField] private Image bar;
    [SerializeField] private Image enemyTurnFill;
    [SerializeField] private Image playerTurnFill;

    public void ToggleEnemyTurnFill(bool isActive)
    {
        enemyTurnFill.gameObject.SetActive(isActive);
    }

    public void TogglePlayerTurnFill(bool isActive)
    {
        playerTurnFill.gameObject.SetActive(isActive);
    }

    public void UpdateEnemyTurnFill(float percent)
    {
        enemyTurnFill.fillAmount = percent;
    }

    public void UpdatePlayerTurnFill(float percent)
    {
        playerTurnFill.fillAmount = percent;
    }
}
