using UnityEngine;

public class FloatingCollectible : MonoBehaviour
{
    [Header("Floating")]
    [SerializeField] private float floatHeight = 0.25f;
    [SerializeField] private float floatSpeed = 2f;
    [SerializeField] private float startingY = 5f;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 90f;

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = new Vector3(transform.position.x, transform.position.y + startingY, transform.position.z);
    }

    private void Update()
    {
        // Float up and down
        float newY = startPosition.y +
            Mathf.Sin(Time.time * floatSpeed) * floatHeight;

        transform.position = new Vector3(
            startPosition.x,
            newY,
            startPosition.z
        );

        // Spin around the Y axis
        transform.Rotate(
            Vector3.up * rotationSpeed * Time.deltaTime,
            Space.World
        );
    }
}