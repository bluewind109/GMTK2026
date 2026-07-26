using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public Action onMenuOpened;

    [SerializeField] private MainMenuInput input;
    [SerializeField] private GameObject buttonsContainer;

    private List<MainMenuButton> buttons = new List<MainMenuButton>();
    private int selectedIndex = 0;
    private int totalActions => buttons.Count;

    void Awake()
    {
        buttons = new List<MainMenuButton>(buttonsContainer.GetComponentsInChildren<MainMenuButton>());

        if (buttons.Count > 0)
        {
            UpdateSelection();
        }
    }

    void Start()
    {
        ShowMenu();
    }

    void OnEnable()
    {
        input.onNavigateUp += OnNavigateUp;
        input.onNavigateDown += OnNavigateDown;
        input.onEnter += OnActionSelect;
        input.onBack += OnBack;
    }

    void OnDisable()
    {
        input.onNavigateUp -= OnNavigateUp;
        input.onNavigateDown -= OnNavigateDown;
        input.onEnter -= OnActionSelect;
        input.onBack -= OnBack;
    }

    void OnDestroy()
    {
        input.onNavigateUp -= OnNavigateUp;
        input.onNavigateDown -= OnNavigateDown;
        input.onEnter -= OnActionSelect;
        input.onBack -= OnBack;
    }

    private void UpdateSelection()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].SetActive(i == selectedIndex);
        }
    }

    public void ShowMenu()
    {
        // this.gameObject.SetActive(true);
        input.ToggleInput(true);
        onMenuOpened?.Invoke();
    }

    public void HideMenu()
    {
        // this.gameObject.SetActive(false);
        input.ToggleInput(false);
    }

    private void OnActionSelect()
    {
        var selectedButton = buttons[selectedIndex];

        switch (selectedButton.ActionType)
        {
            case MainMenuActionType.StartGame:
                Debug.Log("Start Game selected");
                SceneManager.LoadScene("Game");
                break;
            case MainMenuActionType.Options:
                Debug.Log("Options selected");
                break;
            case MainMenuActionType.Exit:
                Debug.Log("Exit selected");
                Application.Quit();
                break;
            default:
                Debug.LogWarning("Unhandled action type: " + selectedButton.ActionType);
                break;
        }

    }

    private void OnBack()
    {
        // HideMenu();
    }


    private void OnNavigateDown()
    {
        if (totalActions == 0) return;

        selectedIndex = (selectedIndex + 1) % totalActions;
        selectedIndex = Mathf.Clamp(selectedIndex, 0, totalActions - 1);
        UpdateSelection();
    }

    private void OnNavigateUp()
    {
        if (totalActions == 0) return;

        selectedIndex = (selectedIndex - 1 + totalActions) % totalActions;
        selectedIndex = Mathf.Clamp(selectedIndex, 0, totalActions - 1);
        UpdateSelection();
    }
}
