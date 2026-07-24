using UnityEngine;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private EnemyConfig enemyConfig;
    [SerializeField] private Transform spawnLocationsParent;
    [SerializeField] private Transform enemiesContainer;
    [SerializeField] private Enemy enemyPrefab;

    private const int MAX_ENEMIES = 20;
    private List<Enemy> enemyPool = new List<Enemy>(MAX_ENEMIES);

    private List<SpawnLocation> spawnLocations;

    void Awake()
    {
        spawnLocations = new List<SpawnLocation>(
            spawnLocationsParent.GetComponentsInChildren<SpawnLocation>()
        );
    }

    void Start()
    {
        for (int i = 0; i < MAX_ENEMIES; i++)
        {
            Enemy enemy = Instantiate(enemyPrefab, enemiesContainer);
            enemy.gameObject.SetActive(false);
            enemy.Initialize(
                enemyConfig.GetMoveSpeed(),
                enemyConfig.GetDamage(),
                enemyConfig.GetAttackRange()
            );
            enemyPool.Add(enemy);
        }
    }

    void Spawn()
    {
        Enemy enemy = GetInactiveEnemy();
        if (enemy == null)
        {
            Debug.Log("<color=red>EnemyManager: No inactive enemies available in the pool.</color>");
        }
        else
        {
            enemy.gameObject.SetActive(true);
        }
    }

    private Enemy GetInactiveEnemy()
    {
        foreach (var enemy in enemyPool)
        {
            if (!enemy.gameObject.activeSelf)
            {
                return enemy;
            }
        }
        return null;
    }
}
