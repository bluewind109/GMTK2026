using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuButton : MonoBehaviour
{
    public static Action onPressed;

    [SerializeField] private MainMenuActionType actionType;
    [SerializeField] private Image activeBg;
    [SerializeField] private TextMeshProUGUI buttonText;

    private bool isActive = false;
    public MainMenuActionType ActionType => actionType;

    public void SetActive(bool active)
    {
        isActive = active;
        activeBg.gameObject.SetActive(active);
        buttonText.color = isActive ? Color.black : Color.white;
    }
}
