using UnityEngine;

public class PigeonManager : MonoBehaviour
{
    [SerializeField] CollisionDetectionScript collisionDetection;
    [SerializeField] PigeonData pigeonData;

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
            Debug.Log("Miam!!");
        }
        else
        {
            Debug.Log("Too Full");
        }
    }
}
