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
		enemyManager.Test_StartLevel1();
	}

	void Update()
	{
		// if (currentPhase == GamePhase.Gameplay)
		// {
		enemyManager.UpdateSpawnLocations();
		enemyManager.UpdateEnemyMovements();
		// }
	}
}

public enum GamePhase
{
	None = -1,
	Start,
	Gameplay,
	End
}
