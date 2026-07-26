using System.Collections;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance { get; private set; }

	[SerializeField] private LevelConfig levelConfig;
	[SerializeField] private EnemyManager enemyManager;
	[SerializeField] private TurnController turnController;
	[SerializeField] private Player player;
	[SerializeField] private Base playerBase;
	[SerializeField] private GameObject introScreen;
	[SerializeField] private TextMeshProUGUI introText;
	[SerializeField] private TextMeshProUGUI levelText;
	[SerializeField] private PopupEndGame popupEndGame;

	private int levelIndex = 0;
	private int numberOfTurns = 1;
	private int numberOfLevels = 1;
	private bool isLastTurn = false;
	private int totalEnemies = 0;
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
		popupEndGame.Hide();
		yield return new WaitForSeconds(1f);
		turnController.Init();
		numberOfLevels = levelConfig.GetNumberOfLevels();

		currentPhase = GamePhase.Start;
		PlayIntro().Forget();
	}

	public async UniTask PlayIntro()
	{
		introText.text = "SURVIVE!";
		AudioManager.Instance.PlayThreeTwoOneCountdownSfx();
		await UniTask.Delay(System.TimeSpan.FromSeconds(1f));
		introText.text = "3";
		AudioManager.Instance.PlayThreeTwoOneCountdownSfx();
		await UniTask.Delay(System.TimeSpan.FromSeconds(1f));
		introText.text = "2";
		AudioManager.Instance.PlayThreeTwoOneCountdownSfx();
		await UniTask.Delay(System.TimeSpan.FromSeconds(1f));
		introText.text = "1";
		AudioManager.Instance.PlayThreeTwoOneCountdownSfx();
		await UniTask.Delay(System.TimeSpan.FromSeconds(1f));
		AudioManager.Instance.PlayZeroCountdownSfx();
		introScreen.gameObject.SetActive(false);

		AudioManager.Instance.PlayBgm();
		totalEnemies = levelConfig.GetTotalEnemies();
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
		ShowLevelText($"Level {index + 1}", 2f).Forget();
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
				_ = EndGame(true);
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

	public async UniTask EndGame(bool isWin)
	{
		currentPhase = GamePhase.End;
		await playerBase.FadeOut();
		popupEndGame.Show(isWin);
	}

	private async UniTask ShowLevelText(string text, float duration)
	{
		levelText.text = text;
		levelText.gameObject.SetActive(true);
		await UniTask.Delay(System.TimeSpan.FromSeconds(duration));
		levelText.gameObject.SetActive(false);
	}

	public void DecreaseTotalEnemies()
	{
		totalEnemies --;
		if (totalEnemies < 0)
		{
			totalEnemies = 0;
			EndGame(true).Forget();
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
