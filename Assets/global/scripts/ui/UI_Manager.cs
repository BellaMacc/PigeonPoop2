
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Manager : MonoBehaviour
{
    [Header("Poop Bar")]
    public Slider poopSlider;
    public TextMeshProUGUI poopBarValueText;

    
    public void UpdatePoopBar(int currentAmount, int totalAmount)
    {
        poopBarValueText.text = currentAmount.ToString() + "/" + totalAmount.ToString();
        poopSlider.value = currentAmount;
        poopSlider.maxValue = totalAmount;
    }



}
