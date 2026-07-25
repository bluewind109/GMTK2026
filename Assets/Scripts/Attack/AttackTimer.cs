using System;
using UnityEngine;

public class AttackTimer
{
    public Action onTimerFinished;

    private float duration;
    private float elapsedTime = 99f;

    public AttackTimer(float duration)
    {
        this.duration = duration;
    }

    public void Start(float duration)
    {
        this.duration = duration;
        elapsedTime = 0f;
    }

    public void Update(float deltaTime)
    {
        if (IsFinished())
        {
            return;
        }
        elapsedTime += deltaTime;
        if (IsFinished())
        {
            onTimerFinished?.Invoke();
        }
    }

    public bool IsFinished()
    {
        return elapsedTime >= duration;
    }
}
