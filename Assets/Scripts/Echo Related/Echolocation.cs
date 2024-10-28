using System;
using UnityEngine;

public class Echolocation : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Material mat;
    private Color color = new Color(1, 1, 1, 1);
    private float timer; //Temporizador que va de timeToDestroy a 0
    private float timeToDestroy;
    private bool isInvisible = false;
    public bool forced = false;
    public bool remaining = false;
    private float parentScale; //Escala del padre

    private void Awake()
    {
        //Cada onda debe tener su propio material para que los cambios en este no afecten a todas
        GetComponent<Renderer>().material = new Material(mat); 
        GameObject.FindObjectOfType<SideChanger>().sideChanged += AutoDestroy; //Se destruyen si se cambia de lado
    }

    private void Start()
    {
        parentScale = transform.parent.localScale.x;
        if (this.transform.position.y > 0) isInvisible = true;
    }

    void Update()
    {
        //Aumento de tamaño:
        float growingSpeed = speed * Time.deltaTime / parentScale; //Dividimos entre la escala para tener velocidad constante en todos los objetos
        if (forced) growingSpeed *= 0.1f;
        transform.localScale += new Vector3(growingSpeed, growingSpeed, growingSpeed);
        //Disminución del alpha para el fade out / invisibilidad:
        if (!isInvisible || forced)
        {
            GetComponent<Renderer>().material.SetColor("_color", new Color(color.r, color.g, color.b, timer / timeToDestroy));
            timer -= Time.deltaTime;
        }
        else GetComponent<Renderer>().material.SetColor("_color", new Color(color.r, color.g, color.b, 0));
    }

    private void AutoDestroy(object s, EventArgs e) //Destrucción manual de la onda
    {
        if(!remaining) Destroy(this.gameObject);
    }

    public void SetDestroy(float t) //Programa la destrucción de la onda cuando se acaba su tiempo de vida
    {
        timeToDestroy = t;
        timer = t;
        Destroy(this.gameObject, t);
    }

    public void SetColor(Color c)
    {
        color = c;
    }

    public Color GetColor()
    {
        return color;
    }

    public bool IsInvisible()
    {
        return isInvisible;
    }

    private void OnDestroy()
    {
        GameObject.FindObjectOfType<SideChanger>().sideChanged -= AutoDestroy;
    }

    public void SetRange(float newRange) //Inicializa la fuerza de pintado de la onda
    {
        GetComponent<Renderer>().material.SetFloat("_range", newRange);
    }
}
