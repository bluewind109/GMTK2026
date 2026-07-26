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
	}

	void OnDestroy()
	{
		if (Instance == this)
		{
			Instance = null;
		}
	}

	void Start()
	{
		// currentPhase = GamePhase.Start;
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
}

public enum GamePhase
{
	None = -1,
	Start,
	Gameplay,
	End
}
