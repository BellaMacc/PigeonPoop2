using UnityEngine;
using UnityEngine.UI;
using TMPro;
//reference: https://www.youtube.com/watch?v=S0gmSDRXzgs

public class poopBar : MonoBehaviour
{
    /*
    public Slider poopSlider;
    public TextMeshProUGUI poopBarValueText;

    public int maxHealth;
    public poopMeter poop;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        poopBarValueText.text = poop.poopNum.ToString() + "/ 5";
        poopSlider.value = poop.poopNum;
        poopSlider.maxValue = maxHealth;


    }*/

    private void OnEnable()
    {
        Debug.Log("poopBar depricated - BM");
    }
}
