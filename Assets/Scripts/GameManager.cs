using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance { get; private set; }

	[SerializeField] private EnemyManager enemyManager;
	[SerializeField] private TurnController turnController;
	[SerializeField] private Player player;

	private int levelIndex = 0;
	private GamePhase currentPhase = GamePhase.None;

	public Player GetPlayer() => player;

	void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(this.gameObject);
			return;
		}
		Instance = this;

		turnController.onPlayerTurnStart += OnPlayerTurnStart;
		turnController.onEnemyTurnStart += OnEnemyTurnStart;
	}

	void OnDestroy()
	{
		if (Instance == this)
		{
			Instance = null;
		}

		turnController.onPlayerTurnStart -= OnPlayerTurnStart;
		turnController.onEnemyTurnStart -= OnEnemyTurnStart;
	}

	IEnumerator Start()
	{
		// currentPhase = GamePhase.Start;
		yield return new WaitForSeconds(1f);
		turnController.Init();
		turnController.StartEnemyTurn();
		enemyManager.Test_StartLevel1();
		currentPhase = GamePhase.Gameplay;
	}

	void Update()
	{
		if (currentPhase == GamePhase.Gameplay)
		{
			float speedMultiplier = turnController.IsPlayerTurn ? 0.1f : 1f;
			enemyManager.UpdateSpawnLocations();
			enemyManager.UpdateEnemyMovements(speedMultiplier);

			turnController.UpdateTurn();
		}
	}

	private void OnPlayerTurnStart()
	{
		player.OnTurnStart();
	}

	private void OnEnemyTurnStart()
	{
		player.OnTurnEnd();
	}
}

public enum GamePhase
{
	None = -1,
	Start,
	Gameplay,
	End
}
