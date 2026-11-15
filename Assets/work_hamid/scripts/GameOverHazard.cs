
using UnityEngine;

public class GameOverHazard : MonoBehaviour
{
    public GameOverManager gameOverManager;
    public GameObject pigeon;
    public string gameOverMessage = "Construction Hazard!";

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == pigeon ||
            other.transform.IsChildOf(pigeon.transform))
        {
            gameOverManager.ShowGameOver(gameOverMessage);
        }
    }
}
