using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance { get; private set; }

	[SerializeField] private LevelConfig levelConfig;
	[SerializeField] private EnemyManager enemyManager;
	[SerializeField] private TurnController turnController;
	[SerializeField] private Player player;

	private int levelIndex = 0;
	private int numberOfTurns = 1;
	private int numberOfLevels = 1;
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
		turnController.onEnemyTurnEnd += OnEnemyTurnEnd;
		turnController.onPlayerTurnEnd += OnPlayerTurnEnd;
	}

	void OnDestroy()
	{
		if (Instance == this)
		{
			Instance = null;
		}

		turnController.onPlayerTurnStart -= OnPlayerTurnStart;
		turnController.onEnemyTurnStart -= OnEnemyTurnStart;
		turnController.onEnemyTurnEnd -= OnEnemyTurnEnd;
		turnController.onPlayerTurnEnd -= OnPlayerTurnEnd;
	}

	IEnumerator Start()
	{
		// currentPhase = GamePhase.Start;
		yield return new WaitForSeconds(1f);
		turnController.Init();
		numberOfLevels = levelConfig.GetNumberOfLevels();
		StartLevel(levelIndex);
	}

	public void StartLevel(int index)
	{
		if (index < 0 || index >= numberOfLevels)
		{
			Debug.LogError($"Invalid level index: {index}. Cannot start level.");
			return;
		}

		Debug.Log($"Starting level {index + 1}");
		LevelInfo levelInfo = levelConfig.GetLevelInfo(index);
		numberOfTurns = levelInfo.numberOfTurns;
		enemyManager.StartLevel(levelInfo);
		turnController.StartEnemyTurn();
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

	private void OnPlayerTurnEnd()
	{
		numberOfTurns--;
		if (numberOfTurns <= 0)
		{
			levelIndex++;
			if (levelIndex >= numberOfLevels)
			{
				Debug.Log("All levels completed!");
				currentPhase = GamePhase.End;
				return;
			}
			StartLevel(levelIndex);
			return;
		}

		turnController.StartEnemyTurn();
	}

	private void OnEnemyTurnStart()
	{
		player.OnTurnEnd();
	}

	private void OnEnemyTurnEnd()
	{
		turnController.StartPlayerTurn();
	}
}

public enum GamePhase
{
	None = -1,
	Start,
	Gameplay,
	End
}
