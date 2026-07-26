using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class TurnController : MonoBehaviour
{
    public Action onEnemyTurnStart;
    public Action onEnemyTurnEnd;
    public Action onPlayerTurnStart;
    public Action onPlayerTurnEnd;

    [SerializeField] private TurnSO config;
    [SerializeField] private float playerDuration = 5f;
    [SerializeField] private TurnTimerUI timerUI;
    [SerializeField] private TextMeshProUGUI countdownText;

    private Turn enemyTurn;
    private Turn playerTurn;
    private Turn currentTurn;

    public bool IsPlayerTurn => currentTurn == playerTurn;
    public bool IsEnemyTurn => currentTurn == enemyTurn;

    private float duration = 1f;

    public void Init()
    {
        duration = config.GetDuration();
        playerDuration = config.GetPlayerDuration();
        enemyTurn = new Turn(this, duration);
        playerTurn = new Turn(this, playerDuration);

        playerTurn.onTurnEnd += OnPlayerTurnEnd;
        enemyTurn.onTurnEnd += OnEnemyTurnEnd;
    }

    void OnDestroy()
    {
        playerTurn.onTurnEnd -= OnPlayerTurnEnd;
        enemyTurn.onTurnEnd -= OnEnemyTurnEnd;
    }

    private void Reset()
    {
        playerTurn.Reset();
        enemyTurn.Reset();
        timerUI.UpdatePlayerTurnFill(playerTurn.TimePercent);
        timerUI.UpdateEnemyTurnFill(enemyTurn.TimePercent);
    }

    private void SetTurn(Turn turn)
    {
        // if (turn == null) return;
        // if (currentTurn == turn) return;

        currentTurn = turn;
        switch (currentTurn)
        {
            case Turn t when t == playerTurn:
                AudioManager.Instance.PlayPlayerTurnSfx();
                timerUI.TogglePlayerTurnFill(true);
                timerUI.ToggleEnemyTurnFill(false);
                onPlayerTurnStart?.Invoke();
                break;
            case Turn t when t == enemyTurn:
                timerUI.TogglePlayerTurnFill(false);
                timerUI.ToggleEnemyTurnFill(true);
                onEnemyTurnStart?.Invoke();
                break;
        }

        currentTurn.Begin();
    }

    public void StartPlayerTurn()
    {
        SetTurn(playerTurn);
    }

    public void StartEnemyTurn()
    {
        SetTurn(enemyTurn);
    }

    public void UpdateTurn()
    {
        if (currentTurn == null) return;

        currentTurn.UpdateDuration();
        UpdateCountdownText();
        if (currentTurn == playerTurn)
        {
            timerUI.UpdatePlayerTurnFill(currentTurn.TimePercent);
        }
        else if (currentTurn == enemyTurn)
        {
            timerUI.UpdateEnemyTurnFill(currentTurn.TimePercent);
        }
    }

    private int currentCountdown = -1;
    private void UpdateCountdownText()
    {
        if (currentTurn == null) return;

        int secondsRemaining = Mathf.CeilToInt(currentTurn.TimePercent * currentTurn.Duration);
        if (secondsRemaining != currentCountdown)
        {
            currentCountdown = secondsRemaining;
            countdownText.text = currentCountdown.ToString();
            PlayCountdownEffect();
            AudioManager.Instance.PlayCountdownSfx();
        }
    }

    private void PlayCountdownEffect()
    {
        countdownText.transform.DOKill();
        Tween countdownTween = countdownText.transform.DOPunchScale(Vector3.one * 1.5f, 0.3f, 1, 0.5f);
    }

    private void OnPlayerTurnEnd()
    {
        onPlayerTurnEnd?.Invoke();
    }

    private void OnEnemyTurnEnd()
    {
        onEnemyTurnEnd?.Invoke();
        Reset();
    }
}
