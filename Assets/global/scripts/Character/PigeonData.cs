using System;
using UnityEngine;

public class PigeonData : MonoBehaviour
{
    [Header("Global References")]
    [SerializeField] CollisionDetectionScript collisionDetection;

    [Header("Stomach Data")]
    [SerializeField] public int maxStomach = 10;
    [SerializeField] public int currentStomach = 0;
    [Header("Money Data")]
    [SerializeField] float moneyTotal = 0f;

    

    public static event Action<int, int> StomachChanged;
    public static event Action<float> MoneyChanged;
    private void Start()
    {
        StomachChanged?.Invoke(currentStomach, maxStomach);
    }
    private void OnEnable()
    {
        collisionDetection.OnEat += HandleEating;
    }

    private void OnDisable()
    {
        collisionDetection.OnEat -= HandleEating;
    }

    public void HandleEating(BasicFoodObject food)
    {
        if (!CanEat()) { return; }

        if (food == null) { return; }

        EatFood(food.GetRefillValue());

        food.Consume();
    }
    public bool EatFood(int amount)
    {
        if (amount + currentStomach <= maxStomach)
        {
            currentStomach += amount;
            StomachChanged?.Invoke(currentStomach, maxStomach);
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
            StomachChanged?.Invoke(currentStomach, maxStomach);
            return true;
           
        }

        //if you can't poop the food return false and take no action on stomach
        return false;
    }

    public void GiveMoney(float amt)
    {
        moneyTotal += amt;
        MoneyChanged?.Invoke(moneyTotal);
    }

    public void TakeMoney(float amt)
    {
        moneyTotal -= amt;
        MoneyChanged?.Invoke(moneyTotal);
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
