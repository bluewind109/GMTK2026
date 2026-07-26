using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PopupEndGame : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button quitButton;

    [SerializeField] private Image resultImage;
    [SerializeField] private Sprite winSprite;
    [SerializeField] private Sprite loseSprite;

    void Awake()
    {
        restartButton.onClick.AddListener(OnRestartButtonClicked);
        quitButton.onClick.AddListener(OnQuitButtonClicked);
    }

    void OnDestroy()
    {
        restartButton.onClick.RemoveListener(OnRestartButtonClicked);
        quitButton.onClick.RemoveListener(OnQuitButtonClicked);
    }

    public void Show(bool isWin)
    {
        AudioManager.Instance.StopBgm();
        gameObject.SetActive(true);
        titleText.text = isWin ? "You Win!" : "Game Over";
        resultImage.sprite = isWin ? winSprite : loseSprite;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnRestartButtonClicked()
    {
        AudioManager.Instance.StopBgm();
        // Reload the current scene
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
    
    private void OnQuitButtonClicked()
    {
        AudioManager.Instance.StopBgm();
        Application.Quit();
    }
}
