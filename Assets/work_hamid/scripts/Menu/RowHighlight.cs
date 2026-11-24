using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Goes on a slider. When the controller selects the slider, the whole settings row
// behind it switches to the gold "focused" card, then back when it's deselected.
public class RowHighlight : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public Image row;          // the white row card behind the slider
    public Sprite normal;      // row_card
    public Sprite focused;     // row_card_focus

    public void OnSelect(BaseEventData eventData)
    {
        if (row != null) row.sprite = focused;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (row != null) row.sprite = normal;
    }

    void OnDisable()
    {
        // Reset when the panel closes so it doesn't stay gold
        if (row != null) row.sprite = normal;
    }
}
