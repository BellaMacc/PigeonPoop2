using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] int points;
    [SerializeField] UI_Manager uiManager;

    public void GivePoints(int amt)
    {
        points += amt;
        uiManager.UpdateScoreBar(points);
    }

    public int GetPoints()
    {
        return points;
    }
}
