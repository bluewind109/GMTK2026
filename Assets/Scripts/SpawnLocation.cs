using UnityEngine;
using System.Collections.Generic;

public class SpawnLocation : MonoBehaviour
{
    [SerializeField] private SpawnAreaConfig spawnAreaConfig;
    private SpriteRenderer spriteRenderer;
    private LevelInfo currentLevelInfo;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        spriteRenderer.enabled = false;
    }

    public void SetLevelInfo(int levelIndex)
    {
        currentLevelInfo = spawnAreaConfig.GetLevelInfo(levelIndex);
    }
}
