using UnityEngine;
using UnityEngine.UI;

public class WarningVignette : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private Image vignetteImage;

    [Header("Vignette Settings")]
    [SerializeField] private Color vignetteColor = Color.red;
    [SerializeField] private float maxAlpha = 0.8f;
    [SerializeField] private float smoothness = 5f;

    [Header("Pulse Settings")] 
    [SerializeField] private float pulseSpeed = 2f;
    [SerializeField] private float pulseStrength = 0.15f;
    [SerializeField] private float scaleStrength = 0.05f;
    [SerializeField] private float scaleSmoothness = 5f;


    [Header("Distance Alert Settings")]
    [SerializeField] private float warningDistnace = 500f;

    private float intensity;
    private float currentAlpha;
    private Vector3 originalScale;
    private Vector3 currentScale;

    private bool effectEnable = false;
    private void Awake()
    {
        originalScale = vignetteImage.rectTransform.localScale;
        currentScale = originalScale;
    }

    private void OnEnable()
    {
        BoundaryDetector.OnFoundBoundary += HandleFoundBoundary;
        BoundaryDetector.OnLostBoundary += HandleLostBoundary;

        VerticalBoundaryDetector.OnHeightLimitDetected += HandleHeightLimit;
        VerticalBoundaryDetector.OnHeightLimitLost += HandleLostBoundary;
    }

    private void OnDisable()
    {
        BoundaryDetector.OnFoundBoundary -= HandleFoundBoundary;
        BoundaryDetector.OnLostBoundary -= HandleLostBoundary;

        VerticalBoundaryDetector.OnHeightLimitDetected -= HandleHeightLimit;
        VerticalBoundaryDetector.OnHeightLimitLost -= HandleLostBoundary;
    }


    private void HandleFoundBoundary(float distance, BoundaryType boundaryType)
    {
        intensity = 1f - Mathf.Clamp01(distance / warningDistnace);
    }
    private void HandleLostBoundary()
    {
        intensity = 0f;
    }
    private void HandleHeightLimit(float distance, BoundaryType limitType)
    {
        intensity = 1f - Mathf.Clamp01(distance / warningDistnace);
    }
    public void SetIntensity(float x)
    {
        intensity = Mathf.Clamp01(x);
    }
    private void Update()
    {
        
        float pulse = (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f;

        // Opacity pulse
        float pulseAmount = pulse * pulseStrength * intensity;
        float targetAlpha = Mathf.Clamp01(intensity * maxAlpha + pulseAmount);

        currentAlpha = Mathf.Lerp(currentAlpha, targetAlpha, smoothness * Time.deltaTime);

        Color color = vignetteColor;
        color.a = currentAlpha;
        vignetteImage.color = color;

        // Scale pulse
        float targetScale = 1f + (pulse * scaleStrength * intensity);
        Vector3 desiredScale = originalScale * targetScale;

        currentScale = Vector3.Lerp(currentScale, desiredScale, scaleSmoothness * Time.deltaTime);
        vignetteImage.rectTransform.localScale = currentScale;
    }
}

