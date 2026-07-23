using UnityEngine;
using UnityEngine.UI;

public class TurnTimerUI : MonoBehaviour
{
    [SerializeField] private Image bar;
    [SerializeField] private Image turnFill;
    [SerializeField] private Image playerTurnFill;

    public void UpdateTurnFill(float percent)
    {
        turnFill.fillAmount = percent;
    }

    public void UpdatePlayerTurnFill(float percent)
    {
        playerTurnFill.fillAmount = percent;
    }
}
