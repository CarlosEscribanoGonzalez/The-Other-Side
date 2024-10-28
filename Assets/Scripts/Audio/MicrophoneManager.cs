using System.Collections;
using UnityEngine;

public class MicrophoneManager : MonoBehaviour
{
    private string micName; //Nombre del micrófono activo
    private AudioClip clip; //Clip que almacena el Input del micro
    private float sensitivity = 40f; //Sensibilidad de detección
    private float currentVolume = 0; //Volumen actual detectado
    private float loudnessThreshold = 0.10f; //Mínimo de volumen para hacer display
    [SerializeField] private float echoInterval; //Intervalos entre echos
    private bool inCooldown = false; //Cooldown de duración echoInterval
    private EchoGenerator echoGenerator;
    private PlayerMovement player;

    void Start()
    {
        SetMicrophone(Microphone.devices[0]);
        echoGenerator = GetComponent<EchoGenerator>();
        player = GameObject.FindObjectOfType<PlayerMovement>();
        //El input del micrófono se añadirá como sonido al juego (generación de eco en el otro mundo):
        //GetComponent<AudioSource>().clip = clip;
        //GetComponent<AudioSource>().Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (!player.GetCanMove()) return;
        currentVolume = GetMicLoudness();
        if (currentVolume > loudnessThreshold && !inCooldown) //Se genera el echo según el intervalo
        {
            inCooldown = true;
            echoGenerator.GenerateEcho(Mathf.Clamp(currentVolume, 0, 0.6f));
            StartCoroutine(Cooldown());
        }
    }

    private float GetMicLoudness()
    {
        float[] samples = new float[128]; //buffer para las muestras de audio
        clip.GetData(samples, Microphone.GetPosition(micName) - samples.Length);
        float sum = 0;
        foreach(float s in samples)
        {
            sum += Mathf.Abs(s);
        }
        return sum/samples.Length * sensitivity;
    }

    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(echoInterval);
        inCooldown = false;
    }

    public void SetSensitivity(float value)
    {
        sensitivity = value;
    }

    public void SetMicrophone(string mic)
    {
        micName = mic;
        clip = Microphone.Start(micName, true, 1, 44100); //Tercer parámetro -> duración en segundos del loop (indicado en true)
                                                          //Cuarto parámetro -> calidad estándar de audio (frecuencia de muestreo)

        Debug.Log("Micrófono en uso: " + micName);
    }

    public string GetMicrophone()
    {
        return micName;
    }
}
