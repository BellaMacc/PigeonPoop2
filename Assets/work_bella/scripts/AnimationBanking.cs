using PlayerLocomotion;
using UnityEngine;

public class AnimationBanking : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Flight flight;
    [SerializeField] private float maxBankAngle = 30f;
    [SerializeField] private float bankSpeed = 5f;

    private float currentBank;

    private void Update()
    {
        float targetBank = 0f;

        if (flight.IsFlying())
        {
            float horizontalInput = inputManager.ReadMovement().x;
            targetBank = -horizontalInput * maxBankAngle;
        }

        currentBank = Mathf.Lerp(
            currentBank,
            targetBank,
            bankSpeed * Time.deltaTime
        );

        transform.localRotation = Quaternion.Euler(
            0f,
            0f,
            currentBank
        );
    }
}

