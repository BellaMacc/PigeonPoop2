
using UnityEngine;

public class FlightBoundary : MonoBehaviour
{
    public GameOverManager gameOverManager;

    public float maxHeight = 150f;
    public float minHeight = -10f;

    private bool gameOverTriggered = false;

    private void Update()
    {
        if (gameOverTriggered) return;

        if (transform.position.y > maxHeight)
        {
            gameOverTriggered = true;
            gameOverManager.ShowGameOver("You Flew Too High!");
        }
        else if (transform.position.y < minHeight)
        {
            gameOverTriggered = true;
            gameOverManager.ShowGameOver("You Flew Too Low!");
        }
    }
}
