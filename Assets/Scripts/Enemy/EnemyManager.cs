using UnityEngine;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private EnemyConfig enemyConfig;
    [SerializeField] private LevelConfig levelConfig;
    [SerializeField] private Transform spawnLocationsParent;
    [SerializeField] private Transform enemiesContainer;
    [SerializeField] private Enemy enemyPrefab;
    [SerializeField] private List<SpawnLocation> spawnLocations;
    [SerializeField] private Transform targetPosition;

    void Awake()
    {
        spawnLocations = new List<SpawnLocation>(
            spawnLocationsParent.GetComponentsInChildren<SpawnLocation>()
        );
    }

    void Start()
    {

    }

    public void UpdateSpawnLocations()
    {
        foreach (var spawnLocation in spawnLocations)
        {
            spawnLocation.UpdateSpawnInterval();
        }
    }
}
