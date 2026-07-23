using UnityEngine;

[CreateAssetMenu(fileName = "TurnSO", menuName = "Configs/TurnSO")]
public class TurnSO : ScriptableObject
{
    [SerializeField] private float duration = 10f;
    [SerializeField] private float playerDuration = 5f;

    public float GetDuration() => duration;
    public float GetPlayerDuration() => playerDuration;
}
