using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UIGlowController : MonoBehaviour
{
    private Image targetImage;
    private Material runtimeMaterial;

    [Header("Glow Settings")]
    [SerializeField] private Color glowColor = Color.white;
    [SerializeField] [Range(0f, 10f)] private float glowIntensity = 2f;

    void Awake()
    {
        targetImage = GetComponent<Image>();

        // If Image has no explicit material (uses default), create one from shader
        Shader shader = Shader.Find("Custom/UI_Glow");
        if (shader == null)
        {
            Debug.LogError("Shader Custom/UI_Glow not found. Make sure the shader file is in the project.");
            return;
        }

        // If targetImage.material is null or is default sprite shader, create new material
        Material baseMat = targetImage.material;
        if (baseMat == null || baseMat.shader == null || baseMat.shader.name == "Sprites/Default")
        {
            runtimeMaterial = new Material(shader);
        }
        else
        {
            // Instantiate existing material so we don't modify shared asset
            runtimeMaterial = new Material(baseMat);
            runtimeMaterial.shader = shader; // ensure uses our shader (keeps other properties)
        }

        targetImage.material = runtimeMaterial;

        ApplyGlow(); // apply initial values
    }

    void OnValidate()
    {
        // Update in editor when values change
        if (runtimeMaterial != null)
        {
            ApplyGlow();
        }
        else
        {
            // If runtimeMaterial not ready but Image exists in editor, try to update the material asset for preview
            if (targetImage == null) targetImage = GetComponent<Image>();
            if (targetImage != null && targetImage.material != null)
            {
                if (targetImage.material.HasProperty("_GlowColor"))
                {
                    targetImage.material.SetColor("_GlowColor", glowColor);
                }
                if (targetImage.material.HasProperty("_GlowIntensity"))
                {
                    targetImage.material.SetFloat("_GlowIntensity", glowIntensity);
                }
                if (targetImage.material.HasProperty("_Tint"))
                {
                    targetImage.material.SetColor("_Tint", glowColor);
                }
            }
        }
    }

    private void ApplyGlow()
    {
        if (runtimeMaterial == null) return;

        if (runtimeMaterial.HasProperty("_GlowColor"))
            runtimeMaterial.SetColor("_GlowColor", glowColor);

        if (runtimeMaterial.HasProperty("_GlowIntensity"))
            runtimeMaterial.SetFloat("_GlowIntensity", glowIntensity);

        // Use same color to tint the sprite so image itself also changes color
        if (runtimeMaterial.HasProperty("_Tint"))
            runtimeMaterial.SetColor("_Tint", glowColor);

        // Optional: ensure Image.color = white so vertex tint doesn't multiply incorrectly
        targetImage.color = Color.white;
    }

    // ===== Public API =====

    public void SetGlowColor(Color color)
    {
        glowColor = color;
        ApplyGlow();
    }

    public void SetGlowIntensity(float intensity)
    {
        glowIntensity = intensity;
        ApplyGlow();
    }

    public void SetGlow(Color color, float intensity)
    {
        glowColor = color;
        glowIntensity = intensity;
        ApplyGlow();
    }
}
