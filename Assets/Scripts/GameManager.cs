using UnityEngine;

public class GameManager : MonoBehaviour
{
	[SerializeField] private EnemyManager enemyManager;
	[SerializeField] private TurnController turnController;

	private int levelIndex = 0;
	private GamePhase currentPhase = GamePhase.None;

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
