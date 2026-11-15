
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverManager : MonoBehaviour
{
    public GameObject gameOverPanel;
    public TMP_Text gameOverReason;

    public void ShowGameOver(string reason)
    {
        gameOverReason.text = reason;
        gameOverPanel.SetActive(true);

        Time.timeScale = 0f;
    }

    public void TryAgain()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }
}
