using System;
using System.Collections;
using UnityEngine;

public class ShaderManager : MonoBehaviour
{
    private bool animate = false;
    [SerializeField] private float lightMultAnimation = 1f;
    private IntensityMultiplier[] mults;
    [SerializeField] private GameObject deathCanvas;
    [Header("Retro Shader:")]
    [SerializeField] private Material retroMaterial; //Material del mundo normal 
    [SerializeField] private float ditherSpread; //Cantidad de "pixelación"
    [SerializeField] private float colorResolution; //Resolución del color
    [Header("Glitch Shader:")]
    [SerializeField] private Material glitchMaterial; //Material del otro lado 
    [SerializeField] private float noiseAmount; //Cantidad de ruido
    [SerializeField] private float glitchStrength; //Fuerza del glitch
    [SerializeField] private float scanlinesStrength; //Líneas horizontales
    private GameObject player;

    void Awake()
    {
        GameObject.FindObjectOfType<SideChanger>().sideChanged += ToggleShaders;
        player = GameObject.FindWithTag("Player");
        GetComponent<Animator>().enabled = false;
        mults = GameObject.FindObjectsOfType<IntensityMultiplier>();
        //Inicializamos los valores buenos:
        ToggleShaders(null, null);
    }

    private void Update()
    {
        if (!animate) return;
        ActivateRetro();
        ActivateGlitch();
        //Se ajusta la luz de la escena:
        float newMult = Mathf.Clamp(lightMultAnimation, 0, mults[0].GetMult());
        foreach (IntensityMultiplier m in mults) if(m != null) m.AdjustLight(newMult, false);
        mults[mults.Length - 1].AdjustLight(newMult, true); //Se hace un último Adjust light que SÍ cambia la luz de la escena
    }

    private void ToggleShaders(object s, EventArgs e)
    {
        if (player.transform.position.y < 0)
        {
            DisableRetro();
            ActivateGlitch();
        }
        else
        {
            ActivateRetro();
            DisableGlitch();
        }
        UpdateAmbientLight();
    }

    public void UpdateAmbientLight()
    {
        if (player.transform.position.y < 0) RenderSettings.ambientIntensity = 0;
        else RenderSettings.ambientIntensity = 0.3f * GameObject.FindObjectOfType<IntensityMultiplier>().GetMult();
        DynamicGI.UpdateEnvironment();
    }

    private void ActivateRetro()
    {
        retroMaterial.SetFloat("_ditherSpread", ditherSpread);
        retroMaterial.SetFloat("_colorResolution", colorResolution);
    }

    private void DisableRetro()
    {
        retroMaterial.SetFloat("_ditherSpread", 0);
        retroMaterial.SetFloat("_colorResolution", 1000000000);
    }

    private void ActivateGlitch()
    {
        glitchMaterial.SetFloat("_noiseAmount", noiseAmount);
        glitchMaterial.SetFloat("_glitchStrength", glitchStrength);
        glitchMaterial.SetFloat("_scanlinesStrength", scanlinesStrength);
    }

    private void DisableGlitch()
    {
        glitchMaterial.SetFloat("_noiseAmount", 0);
        glitchMaterial.SetFloat("_glitchStrength", 0);
        glitchMaterial.SetFloat("_scanlinesStrength", 0);
    }

    public void OnAnimationStarted() //Comienzo de la animación de cambio de mundo
    {
        animate = true;
    }

    public void OnAnimationEnded()
    {
        StartCoroutine(ChangeScene());
    }

    public void ForceEcho()
    {
        GameObject.Find("EchoForcer").GetComponent<EchoGenerator>().GenerateEcho(0.5f);
    }

    IEnumerator ChangeScene()
    {
        deathCanvas.SetActive(true);
        GameObject.FindObjectOfType<AudioManager>().ToggleListeners();
        yield return new WaitForSeconds(5);
        GameObject.FindObjectOfType<SceneChanger>().ChangeScene();
    }
}
