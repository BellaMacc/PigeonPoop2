using UnityEngine;

public class PigeonData : MonoBehaviour
{
    [Header("Global References")]
    [SerializeField] UI_Manager uiManager;
    [Header("Stomach Data")]
    [SerializeField] int maxStomach = 10;
    [SerializeField] int currentStomach = 0;

    private void Start()
    {
        //uiManager.UpdatePoopBar(10, maxStomach);
        
        // Hamid Changes:
        // Show the real starting amount (was hard-coded to 10, so the bar started full)
        uiManager.UpdatePoopBar(currentStomach, maxStomach);
    }
    public bool EatFood(int amount)
    {
        if (amount + currentStomach <= maxStomach)
        {
            currentStomach += amount;
            uiManager.UpdatePoopBar(currentStomach, maxStomach);
            return true;
        }

        //if you can't eat the food return false and take no action on stomach
        return false; ;
    }

    public bool PoopFood (int amount)
    {
        if (currentStomach - amount >= 0)
        {
            currentStomach -= amount;
            uiManager.UpdatePoopBar(currentStomach, maxStomach);
            return true;
           
        }

        //if you can't poop the food return false and take no action on stomach
        return false;
    }
    public bool CanEat()
    {
        return currentStomach < maxStomach;
    }

    public bool CanEat(int amount)
    {
        return currentStomach + amount <= maxStomach;
    }


    public bool CanPoop()
    {
        return currentStomach > 0;
    }

    
}
