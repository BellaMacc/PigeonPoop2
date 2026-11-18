using UnityEngine;
/*
 * Handles Connections between differnt classes
 * Handles sound, checks and calling functionalities
 * */
public class PigeonManager : MonoBehaviour
{
    [SerializeField] CollisionDetectionScript collisionDetection;
    [SerializeField] PigeonData pigeonData;
    [SerializeField] RandomFoodSpawner foodSpawner;
    [SerializeField] PlayerLocomotion.Flight flight;
    

    [SerializeField] AudioSource audioSource;

    [SerializeField] private int points = 0;


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
            foodSpawner.SubtractFoodFromField(1);
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

    public bool IsGrounded()
    {
        return flight.IsGrounded;
    }


    public void GivePoints(int amt)
    {
        points += amt; 
    }

    public int GetPoints() { return points; }
}
