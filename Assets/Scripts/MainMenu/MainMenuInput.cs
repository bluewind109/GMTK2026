using UnityEngine;
using System;

public class MainMenuInput : MonoBehaviour
{
    public Action onNavigateUp;
    public Action onNavigateDown;
    public Action onBack;
    public Action onEnter;

    private bool isEnabled = false;

    public void ToggleInput(bool enabled)
    {
        isEnabled = enabled;
    }

    void Update()
    {
        if (!isEnabled) return;
        if (!IsAllowedInput()) return;

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            onNavigateUp?.Invoke();
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            onNavigateDown?.Invoke();
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            onEnter?.Invoke();
        }
        else if (Input.GetKeyDown(KeyCode.Backspace))
        {
            onBack?.Invoke();
        }
    }

    private bool IsAllowedInput()
    {
        return Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) ||
               Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow) ||
               Input.GetKeyDown(KeyCode.Return) ||
               Input.GetKeyDown(KeyCode.Backspace);
    }
}

public enum MainMenuActionType
{
    StartGame,
    Options,
    Exit
}
