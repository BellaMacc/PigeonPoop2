
using System.Collections.Generic;
using UnityEngine;


public class RandomFoodSpawner : MonoBehaviour
{
    [Header("Food Settings")]
    [SerializeField] private GameObject foodPrefab;
    [SerializeField] private int foodOnField;
    [SerializeField] private int maxFoodOnField = 10;

    [Header("Spawn Locations")]
    [SerializeField] private List<BoxCollider> spawnAreas = new List<BoxCollider>();

    private void Update()
    {
        if (foodOnField < maxFoodOnField)
        {
            SpawnFood();
        }
    }

    private void SpawnFood()
    {
        if (spawnAreas.Count == 0 || foodPrefab == null)
            return;

        BoxCollider selectedArea = spawnAreas[Random.Range(0, spawnAreas.Count)];

        if (selectedArea == null)
            return;

        Vector3 halfSize = selectedArea.size * 0.5f;

        float randomX = Random.Range(-halfSize.x, halfSize.x);
        float randomZ = Random.Range(-halfSize.z, halfSize.z);

        Vector3 localSpawnPosition = new Vector3(
            randomX,
            -halfSize.y,
            randomZ
        );

        Vector3 spawnPosition = selectedArea.transform.TransformPoint(
            selectedArea.center + localSpawnPosition
        );

        Instantiate(foodPrefab, spawnPosition, Quaternion.identity);

        foodOnField++;
    }

    public void SubtractFoodFromField(int amt)
    {
        foodOnField -= amt;
    }
}