using UnityEngine;
//https://www.youtube.com/watch?v=IbiwNnOv5So
public class randomFoodSpawner : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject foodPrehab;
    public int foodOnField;
    // Update is called once per frame
    void Update()
    {
        if (foodOnField<10)
        {
            Vector3 randomSpawnPosition = new Vector3(Random.Range(-10, 11), 5, Random.Range(-10, 11));
            Instantiate(foodPrehab, randomSpawnPosition, Quaternion.identity);
            foodOnField++;

        }
    }
}
