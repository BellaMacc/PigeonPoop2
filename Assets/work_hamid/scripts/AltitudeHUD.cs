using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Drives the vertical altitude bar from the pigeon's height.
// Uses a direct reference to the pigeon (no Player tag).
public class AltitudeHUD : MonoBehaviour
{
    [Header("Pigeon")]
    // Drag the pigeon here (the CharacterController object in the Hierarchy)
    public Transform pigeon;

    [Header("Height Range")]
    // Keep these the same as FlightBoundary so the bar's ends = Game Over heights
    public float minHeight = -10f;
    public float maxHeight = 150f;

    [Header("UI")]
    public Image altitudeFill;            // Filled, Vertical, Bottom origin
    public RectTransform marker;          // Arrow that slides up and down beside the bar
    public TextMeshProUGUI altitudeText;  // "62 m" label

    void Update()
    {
        if (pigeon == null) return;

        float y = pigeon.position.y;

        // 0 at minHeight, 1 at maxHeight (clamped, so it never leaves the bar)
        float t = Mathf.InverseLerp(minHeight, maxHeight, y);

        // Fill the bar to the current height
        if (altitudeFill != null)
            altitudeFill.fillAmount = t;

        // Slide the arrow: anchoring it at height t keeps it level with the fill
        if (marker != null)
        {
            marker.anchorMin = new Vector2(0f, t);
            marker.anchorMax = new Vector2(0f, t);
        }

        // Show the height as a whole number
        if (altitudeText != null)
            altitudeText.text = Mathf.RoundToInt(y) + " m";
    }
}