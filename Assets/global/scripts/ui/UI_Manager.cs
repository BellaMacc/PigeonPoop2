
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Manager : MonoBehaviour
{
    [Header("Poop Bar")]
    public Slider poopSlider;
    public TextMeshProUGUI poopBarValueText;

    [Header("Score Bar")]
    public TextMeshProUGUI scoreValue;

    // Hamid Chnages:
    [Header("Stomach Meter")]
    // Green liquid Image (Image Type: Filled, Vertical, Bottom)
    public Image stomachLiquid;
    // Black "6/10" text that sits inside the stomach
    public TextMeshProUGUI stomachCount;
    // The stomach body doesn't fill the whole PNG (the tube sits on top),
    // so these map "empty" and "full" to where the body actually starts and ends
    [Range(0f, 1f)] public float liquidEmptyFill = 0.07f;
    [Range(0f, 1f)] public float liquidFullFill = 1f;

    public void UpdatePoopBar(int currentAmount, int totalAmount)
    {
        //poopBarValueText.text = currentAmount.ToString() + "/" + totalAmount.ToString();
        //poopSlider.value = currentAmount;
        //poopSlider.maxValue = totalAmount;

        if (poopSlider != null)
        {
            poopSlider.maxValue = totalAmount;
            poopSlider.value = currentAmount;
        }
        if (poopBarValueText != null)
            poopBarValueText.text = currentAmount + "/" + totalAmount;

        // New stomach meter: liquid level shows how full the stomach is
        if (stomachLiquid != null && totalAmount > 0)
        {
            float percent = (float)currentAmount / totalAmount;            // 0 = empty, 1 = full
            stomachLiquid.fillAmount = Mathf.Lerp(liquidEmptyFill, liquidFullFill, percent);
        }
        if (stomachCount != null)
            stomachCount.text = currentAmount + "/" + totalAmount;
    }

    public void UpdateScoreBar(int currentScore)
    {
        //scoreValue.text = currentScore.ToString() + ".00 $";

        //Hamid Changes for Piggy bank
        scoreValue.text = "$" + currentScore + ".00";
    }



}
