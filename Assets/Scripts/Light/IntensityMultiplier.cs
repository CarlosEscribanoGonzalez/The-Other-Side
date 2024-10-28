using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntensityMultiplier : MonoBehaviour
{
    private Light thisLight;
    private float initialIntensity;
    private static float mult = 1;
    private ShaderManager shaderManager;

    private void Awake()
    {
        thisLight = GetComponent<Light>();
        initialIntensity = thisLight.intensity;
        shaderManager = GameObject.FindObjectOfType<ShaderManager>();
    }

    public void AdjustLight(float m, bool updateAmbient)
    {
        mult = m;
        thisLight.intensity = initialIntensity*m;
        if(updateAmbient) shaderManager.UpdateAmbientLight();
    }

    public float GetMult()
    {
        return mult;
    }
}
