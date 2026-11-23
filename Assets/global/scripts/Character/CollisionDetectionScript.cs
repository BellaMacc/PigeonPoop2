using System;
using Unity.Cinemachine;
using UnityEngine;

public class CollisionDetectionScript : MonoBehaviour
{

    public event Action<BasicFoodObject> OnEat;
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        BasicFoodObject food = hit.collider.GetComponent<BasicFoodObject>();

        if (food != null)
        {
            Debug.Log("I found food");
            OnEat?.Invoke(food);
        }
    }
}
