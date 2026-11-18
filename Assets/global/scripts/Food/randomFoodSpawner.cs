
using System.Collections.Generic;
using UnityEngine;
public class RandomFoodSpawner : MonoBehaviour
{
    [ Header("Food Settings")]
    [SerializeField] private GameObject foodPrefab;
    [SerializeField] private int foodOnField;
    [SerializeField] private int maxFoodOnField = 10;

    [Header("Spawn Locations")]
    [SerializeField] private Transform spawnLocationsParent;

    private List<Transform> spawnPoints = new List<Transform>();

    private void Start()
    {
        PopulateSpawnPoints();
    }

    private void PopulateSpawnPoints()
    {
        spawnPoints.Clear();

        foreach (Transform spawnPoint in spawnLocationsParent)
        {
            spawnPoints.Add(spawnPoint);
        }
    }

    private void Update()
    {
        if (foodOnField < maxFoodOnField)
        {
            SpawnFood();
        }
    }

    private void SpawnFood()
    {
        if (spawnPoints.Count == 0 || foodPrefab == null)
            return;

        Transform selectedPoint = spawnPoints[
            Random.Range(0, spawnPoints.Count)
        ];

        Instantiate(
            foodPrefab,
            selectedPoint.position,
            selectedPoint.rotation
        );

        foodOnField++;
    }

    public void SubtractFoodFromField(int amt)
    {
        foodOnField -= amt;
    }
}