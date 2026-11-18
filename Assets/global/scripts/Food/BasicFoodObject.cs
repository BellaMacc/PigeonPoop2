using UnityEngine;

public class BasicFoodObject : MonoBehaviour
{
    [SerializeField] private string foodName = "Default";
    [SerializeField] int refillValue = 1;

    
    public int GetRefillValue()
    {
        return refillValue;
    }

    public void Consume()
    {
        Destroy(gameObject);
    }
}
