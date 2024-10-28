using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EchoGenerator : MonoBehaviour
{
    public GameObject echoPrefab;
    [SerializeField] private float interval; //Intervalo entre sucesivas ondas de echo
    [SerializeField] private float decreasingFactor; //Las sucesivas ondas se pintan con menor fuerza (exponencial)
    [SerializeField] private float numWaves = 1; //Número de ondas por cada sonido
    [SerializeField] private Color color = new Color(1,1,1,1); //Color de las ondas generadas
    private float initialRange; //Potencia de la primera onda (fuerza con la que se pinta, no distancia a la que llega)
    private float timeToDestroy; //Tiempo para que desaparezca la onda
    [SerializeField] private bool forceEcho = false;
    [SerializeField] private bool remainingEcho = false;

    private void Awake()
    {
        GameObject.FindObjectOfType<SideChanger>().sideChanged += StopEcho; //Al cambiar de lado se para la generación de echo
    }

    public void GenerateEcho(float soundIntensity)
    {
        initialRange = Mathf.Clamp(1 - soundIntensity, 0.15f, 1); //Menos range -> mayor fuerza de pintado
        timeToDestroy = soundIntensity; //Sonidos altos -> mayor tiempo de destrucción
        StartCoroutine(Echo());
    }

    private void StopEcho(object s, EventArgs e)
    {
        if(!remainingEcho) StopAllCoroutines();
    }

    IEnumerator Echo()
    {
        //Se almacena en esta función initialRange y timeToDestroy,
        //pues de esta forma el mismo generador puede ir pintando otras ondas simultáneamente en caso de que haga falta
        float initRange = initialRange;
        float timer = timeToDestroy;
        for(int i = 0; i < numWaves; i++)
        {
            Echolocation newEcho = GameObject.Instantiate(echoPrefab, this.transform).GetComponent<Echolocation>();
            newEcho.SetColor(color);
            newEcho.SetRange(initRange * Mathf.Pow(decreasingFactor, i));
            newEcho.SetDestroy(timer);
            if (forceEcho) newEcho.forced = true;
            if (remainingEcho) newEcho.remaining = true;
            yield return new WaitForSeconds(interval);
        }
    }

    public void SetColor(Color color)
    {
        this.color = color;
    }
}
