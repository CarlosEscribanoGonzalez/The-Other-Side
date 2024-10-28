using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ATakeableObject : MonoBehaviour, ITakeableItem
{
    [SerializeField] private Material mat; //Material Fresnel para que el objeto se ilumine al ser visto
    [SerializeField] private Texture texture; //Textura del objeto
    [SerializeField] private Color colorMult = new Color(1,1,1,1); //Multiplicador de color del objeto (puzles colores...)
    [SerializeField] private GameObject text; //Texto que indica que se puede coger
    [SerializeField] private Vector3 textOffset; //Offset para centrar el texto
    [SerializeField] private AudioSource collisionSource; //AudioSource de la colisión del objeto al chocar contra otra cosa
    protected AudioSource source;
    protected EchoGenerator echoGenerator;
    protected PlayerMovement player;
    [Header("On hand transform: ")]
    [SerializeField] private Vector3 onHandPosition;
    [SerializeField] private Vector3 onHandRotation;
    protected bool canBeUsed = false; //Indica si un objeto puede ser usado (está en la mano)
    private Animator anim;
    protected PickUpController controller; 
    private Camera cam;

    protected virtual void Awake()
    {
        GetComponent<Renderer>().materials[0] = new Material(mat); //Se crea un nuevo material por cada objeto para evitar que los cambios en el mismo repercutan al resto
        GetComponent<Renderer>().material.SetTexture("_texture", texture);
        GetComponent<Renderer>().material.SetColor("_colorMult", colorMult);
        source = GetComponent<AudioSource>();
        echoGenerator = GetComponent<EchoGenerator>();
        InitializeText();
        cam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        anim = GetComponent<Animator>();
        controller = GameObject.FindObjectOfType<PickUpController>();
        player = GameObject.FindObjectOfType<PlayerMovement>();
    }

    private void InitializeText()
    {
        text.GetComponent<TextMeshPro>().text = $"Coger {gameObject.name}";
        text.SetActive(false); //El texto sólo se ve cuando se hace hover al objeto
    }

    protected virtual void Update()
    {
        if(!player.GetCanMove()) text.SetActive(false);
        text.transform.forward = text.transform.position - cam.transform.position; //El texto siempre apunta a la cámara
        text.transform.position = text.transform.parent.position + textOffset; //Se ajusta la posición independientemente de la rotación del objeto
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            TryUse(); //Cuando se pulsa el click izquierdo el objeto intenta ser usado
        }
    }

    public virtual void HoverEnter() //Se activa el texto y la iluminación
    {
        anim.SetBool("HoverEnter", true);
        anim.SetBool("HoverExit", false);
        anim.SetBool("Scanned", false);
        text.SetActive(true);
    }

    public virtual void HoverExit() //Se desactivan el texto y la iluminación
    {
        anim.SetBool("HoverExit", true);
        anim.SetBool("HoverEnter", false);
        text.SetActive(false);
    }

    public virtual void Take() 
    {
        //Se desactivan las colisiones y físicas del objeto
        this.GetComponent<Rigidbody>().velocity = this.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        this.GetComponent<Rigidbody>().useGravity = false;
        this.GetComponent<BoxCollider>().enabled = false;
        //El objeto pasa a la posición requerida, teniendo como padre ahora a "GrabTransform", fijándolo a la cámara
        this.transform.parent = GameObject.Find("GrabTransform").transform;
        this.transform.position = transform.parent.position;
        this.transform.rotation = transform.parent.rotation;
        //Se pone en las coordenadas deseadas para que se vea bien, particulares del objeto:
        if(onHandPosition != Vector3.zero) this.transform.localPosition = onHandPosition;
        if(onHandRotation != Vector3.zero) this.transform.localRotation = Quaternion.Euler(onHandRotation);
        //El objeto ahora puede ser usado
        canBeUsed = true;
    }

    public virtual void Throw()
    {
        //Se reactivan las físicas
        this.GetComponent<Rigidbody>().useGravity = true;
        this.GetComponent<BoxCollider>().enabled = true;
        //Se desfija el objeto de la cámara
        this.transform.parent = null;
        //Se le añade una fuerza hacia delante para simular un lanzamiento
        GetComponent<Rigidbody>().AddForce(cam.transform.forward * 2f, ForceMode.Impulse);
        //El objeto ya no puede ser usado
        canBeUsed = false;
    }

    public void TryUse()
    {
        if (canBeUsed && player.GetCanMove()) Use(); //Si puede ser usado se usa
    }

    protected virtual void Use() { }


    protected virtual void OnTriggerEnter(Collider other) //Al ser tocado por el echo se hace la animación
    {
        if (other.CompareTag("Echo") && !canBeUsed) //Si está en la mano no se ilumina
        {
            if(!other.GetComponent<Echolocation>().IsInvisible()) anim.SetBool("Scanned", true);
        }
    }

    public void ScanFinished() //Llamado por la animación de scan al terminar
    {
        anim.SetBool("Scanned", false);
    }

    private void OnCollisionEnter(Collision collision) //Reproduce audio de choque cuando choca con algo
    {
        collisionSource.Play();
        echoGenerator.GenerateEcho(collisionSource.volume/2);
    }

    public GameObject GetGameObject()
    {
        return this.gameObject;
    }
}
