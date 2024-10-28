using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightPanel : MonoBehaviour
{
    [SerializeField] private Renderer[] lightIndicators; //Indicadores que se encienden para enseñar cuántas luces están encendidas
    [SerializeField] private Material onMaterial; //Igual que las bombillas los materiales
    [SerializeField] private Material offMaterial;
    [SerializeField] private ColorDoor door;
    private LightBulb[] bulbs; //Todas las bombillas
    private int index = 0;
    public bool completed = false;

    private void Awake()
    {
        bulbs = GameObject.FindObjectsOfType<LightBulb>();
    }

    public void TurnOnLight()
    {
        lightIndicators[index].material = onMaterial;
        index++;
        if (index == lightIndicators.Length)
        {
            foreach (LightBulb b in bulbs) { b.StopAllCoroutines(); }
            completed = true;
            door.DissolveDoor();
            if (this.transform.position.y < 0) Notepad.AddMaxDisplay(); //El if para que sólo lo haga uno de los paneles
            foreach (SoundRepeater repeater in GameObject.FindObjectsOfType<SoundRepeater>()) repeater.TurnOff();
            AudioSource source = GetComponent<AudioSource>();
            source.Play();
            GetComponent<EchoGenerator>().GenerateEcho(source.volume);
        }
    }

    public void TurnOffLight()
    {
        index--;
        lightIndicators[index].material = offMaterial;
    }
}
