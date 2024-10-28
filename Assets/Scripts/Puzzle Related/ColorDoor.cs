using UnityEngine;
using UnityEngine.AI;

public class ColorDoor : MonoBehaviour
{
    [SerializeField] private Color doorColor; //Color de la puerta
    [SerializeField] private Color edgeColor; //Color de los bordes
    [SerializeField] private Material disolveMat; //Material de disolución
    [SerializeField] private ColorDoor otherDoor; //Puerta asociada en el otro mundo
    public Animator anim;
    private Renderer rend;
    
    void Awake()
    {
        rend = GetComponent<Renderer>();
        rend.material = new Material(disolveMat);
        rend.material.SetColor("_albedo", doorColor);
        rend.material.SetColor("_edgeColor", edgeColor);
        anim = GetComponent<Animator>();
        anim.speed = 0; //Sólo hay una animación en el animator, así que se pausa a la espera de que se quiera reproducir
    }

    public void DissolveDoor() //Abre también la puerta del otro aldo
    {
        if (this.name.Equals("GreenDoor") && this.transform.position.y < 0 && anim.speed != 1) Notepad.AddMaxDisplay();
        if (otherDoor != null) otherDoor.anim.speed = 1;
        anim.speed = 1;
    }

    private void DisableCollider() //Llamada desde la animación
    {
        GetComponent<Collider>().enabled = false;
        GetComponent<NavMeshObstacle>().enabled = false;
    }

    private void OnTriggerEnter(Collider other) //Si la onda de echo es del mismo color que la puerta esta se abre
    {
        if (other.CompareTag("Echo"))
        {
            Color echoColor = other.GetComponent<Echolocation>().GetColor();
            if (echoColor == doorColor)
            {
                DissolveDoor();
            }
        }
    }

    public void PlaySFX()
    {
        GetComponent<AudioSource>().Play();
    }
}
