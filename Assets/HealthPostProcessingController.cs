using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class HealthPostProcessingController : MonoBehaviour
{
    public Volume globalVolume;
    public string playerHealthVariableName = "playerHealth";
    public AnimationCurve chromaticAberrationCurve = new AnimationCurve(new Keyframe(0, 0.35f), new Keyframe(16, 0.125f));
    public AnimationCurve vignetteCurve = new AnimationCurve(new Keyframe(0, 0.1f), new Keyframe(16, 0.0f));

    private float playerHealth;
    private ChromaticAberration chromaticAberration;
    private Vignette vignette;

    void Start()
    {
        if (globalVolume == null)
        {
            globalVolume = GetComponent<Volume>();
            if (globalVolume == null)
            {
                enabled = false;
                return;
            }
        }

        if (globalVolume.profile.TryGet(out chromaticAberration))
        {
            if (chromaticAberration == null)
            {
                Debug.LogError("Chromatic Aberration is null");
                enabled = false;
                return;
            }
        }
        else
        {
            Debug.LogError("Chromatic Aberration effect not found.");
            enabled = false;
            return;
        }

        if (globalVolume.profile.TryGet(out vignette))
        {
            if (vignette == null)
            {
                Debug.LogError("Vignette is null.");
                enabled = false;
                return;
            }
        }
        else
        {
            Debug.LogError("Vignette effect not found.");
            enabled = false;
            return;
        }
    }

    void Update()
    {
        try
        {
            object variableValue = Unity.VisualScripting.Variables.ActiveScene.Get(playerHealthVariableName);
            if (variableValue != null)
            {
                if (variableValue is float)
                {
                    playerHealth = (float)variableValue;
                }
                else if (variableValue is int)
                {
                    playerHealth = (int)variableValue;
                }
                else
                {
                    Debug.LogError($"Unexpected type for playerHealth variable: {variableValue.GetType()}.");
                    enabled = false;
                    return;
                }
            }
            else
            {
                Debug.LogError($"playerHealth variable is null.");
                enabled = false;
                return;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error getting playerHealth variable: {e.Message}.");
            enabled = false;
            return;
        }

        float chromaticAberrationIntensity = chromaticAberrationCurve.Evaluate(playerHealth);
        float vignetteIntensity = vignetteCurve.Evaluate(playerHealth);

        chromaticAberration.intensity.value = chromaticAberrationIntensity;
        vignette.intensity.value = vignetteIntensity;
    }
}
