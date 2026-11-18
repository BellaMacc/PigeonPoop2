using System;
using Unity.Cinemachine;
using UnityEngine;

public class CollisionDetectionScript : MonoBehaviour
{

    public static event Action<BasicFoodObject> OnEat;
    private void OnTriggerEnter(Collider other)
    {
       if (other.GetComponent<BasicFoodObject>() != null)
        {
            Debug.Log("I found food");
            BasicFoodObject food = other.GetComponent<BasicFoodObject>();
            OnEat?.Invoke(food);
        }
    }
}
