using UnityEngine;

public class PigeonManager : MonoBehaviour
{
    [SerializeField] CollisionDetectionScript collisionDetection;
    [SerializeField] PigeonData pigeonData;
    [SerializeField] randomFoodSpawner foodSpawner;
    [SerializeField] PlayerLocomotion.Flight flight;

    [SerializeField] AudioSource audioSource;




    private void OnEnable()
    {
        CollisionDetectionScript.OnEat += HandleEating;
    }
    private void OnDisable()
    {
        CollisionDetectionScript.OnEat -= HandleEating;
    }


    public void HandleEating(BasicFoodObject food)
    {
        Debug.Log("Food is being assessed");
        if (pigeonData.CanEat(food.GetRefillValue()))
        {
            pigeonData.EatFood(food.GetRefillValue());
            food.Consume();
            foodSpawner.foodOnField -= 1;
            SoundFXManager.PlaySound3D(SoundType.EAT, transform);
            Debug.Log("Miam!!");
        }
        else
        {
            Debug.Log("Too Full");
        }
    }
    public void HandlePoop()
    {
        pigeonData.PoopFood(1);
        SoundFXManager.PlaySound3D(SoundType.POOP, transform);
    }
    public bool CanPoop()
    {
        return (pigeonData.CanPoop() && !flight.IsGrounded);    
    }


}
