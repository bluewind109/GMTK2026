using UnityEngine;
using System;

public class SpawnLocation : MonoBehaviour
{
    public Action<EnemyType, Vector3> onIntervalReached;

    [SerializeField] private float spawnRadius = 0.5f;

    private SpriteRenderer spriteRenderer;
    private float spawnInterval = 0.5f;
    private float spawnTimer = 0f;
    private int spawnAmount = 7;
    private SpawnAreaInfo spawnAreaInfo;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        // spriteRenderer.enabled = false;
    }

    private void ResetTimer()
    {
        spawnTimer = UnityEngine.Random.Range(spawnInterval * 0.9f, spawnInterval * 1.1f);
    }

    public void Initialize(SpawnAreaInfo areaInfo, float interval)
    {
        spawnInterval = interval;
        spawnAreaInfo = areaInfo;
        spawnAmount = areaInfo.spawnAmount;
        ResetTimer();
    }

    public void UpdateSpawnInterval()
    {
        if (spawnTimer <= 0f) return;

        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f)
        {
            ResetTimer();
            SetupSpawn();
        }
    }

    private void SetupSpawn()
    {
        if (spawnAmount <= 0) return;

        Vector3 spawnPosition =
            transform.position +
            (Vector3)(UnityEngine.Random.insideUnitCircle.normalized * spawnRadius);

        EnemyType enemyType = spawnAreaInfo.GetRandomEnemyType();

        spawnAmount--;
        onIntervalReached?.Invoke(enemyType, spawnPosition);
    }
}
