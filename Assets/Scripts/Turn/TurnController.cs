using UnityEngine;

public class TurnController : MonoBehaviour
{
    [SerializeField] private TurnSO config;
    [SerializeField] private float playerDuration = 5f;
    [SerializeField] private TurnTimerUI timerUI;

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
        timerUI.UpdateTurnFill(enemyTurn.TimePercent);
        StartPlayerTurn();
    }

    private void SetTurn(Turn turn)
    {
        if (currentTurn == turn) return;

        currentTurn = turn;
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

    void Update()
    {
        if (currentTurn == null) return;

        currentTurn.UpdateDuration();
        if (currentTurn == playerTurn)
        {
            timerUI.UpdatePlayerTurnFill(currentTurn.TimePercent);
        }
        else if (currentTurn == enemyTurn)
        {
            timerUI.UpdateTurnFill(currentTurn.TimePercent);
        }
    }

    private void OnPlayerTurnEnd()
    {
        StartEnemyTurn();
    }

    private void OnEnemyTurnEnd()
    {
        Reset();
    }
}
