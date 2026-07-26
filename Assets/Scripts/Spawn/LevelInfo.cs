using UnityEngine;

[CreateAssetMenu(fileName = "LevelInfo", menuName = "Configs/Level/LevelInfo")]
public class LevelInfo : ScriptableObject
{
    public float spawnInterval = 0.5f;
    public int numberOfTurns = 3;

    public SpawnAreaInfo leftSpawn;
    public SpawnAreaInfo centerSpawn;
    public SpawnAreaInfo rightSpawn;
}

