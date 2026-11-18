
using UnityEngine;

public class GameOverHazard : MonoBehaviour
{
    public GameOverManager gameOverManager;
    public string gameOverMessage = "Construction Hazard!";

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            gameOverManager.ShowGameOver(gameOverMessage);
        }
    }
}
