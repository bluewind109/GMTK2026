using UnityEngine;
using System;

public class Turn
{
    public Action onTurnBegin;
    public Action onTurnEnd;

    private TurnController controller;
    private float duration = 1f;
    private float currentTime = 0f;
    public float TimePercent
    {
        get
        {
            if (duration <= 0f) return 0f;
            return Mathf.Clamp01(currentTime / duration);
        }
    }

    private bool isFinished = false;

    public Turn(TurnController _controller, float _duration)
    {
        controller = _controller;
        duration = _duration;
        currentTime = duration;
        isFinished = false;
    }

    public void Reset()
    {
        currentTime = duration;
    }

    public void Begin()
    {
        onTurnBegin?.Invoke();
        currentTime = duration;
        isFinished = false;
    }

    public void End()
    {
        if (isFinished) return;
        isFinished = true;
        onTurnEnd?.Invoke();
    }

    public void UpdateDuration()
    {
        if (isFinished) return;

        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
        }
        else
        {
            currentTime = 0;
            End();
        }
    }

    public float Duration => duration;
}
