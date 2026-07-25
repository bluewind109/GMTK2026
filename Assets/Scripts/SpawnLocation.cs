using UnityEngine;
using System;

public class SpawnLocation : MonoBehaviour
{
    public Action<Vector3> onIntervalReached;

    [SerializeField] private float spawnRadius = 0.5f;

    private SpriteRenderer spriteRenderer;
    private float spawnInterval = 0.5f;
    private float spawnTimer = 0f;
    private int spawnAmount = 7;

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

    public void Initialize(float interval, int amount)
    {
        spawnInterval = interval;
        spawnAmount = amount;
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
        Vector3 spawnPosition =
            transform.position +
            (Vector3)(UnityEngine.Random.insideUnitCircle.normalized * spawnRadius);
        if (spawnAmount <= 0) return;
        spawnAmount--;
        onIntervalReached?.Invoke(spawnPosition);
    }
}
