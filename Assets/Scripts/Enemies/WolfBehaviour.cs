using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class WolfBehaviour : MonoBehaviour, IWolf
{
    private AWolfState currentState;
    private Animator anim;
    private NavMeshAgent agent; //NavMeshAgent
    private GameObject player;
    [SerializeField] private float sightAngle; //�ngulo de visi�n
    [SerializeField] private Transform eyesTransform; //Posici�n de los ojos
    [SerializeField] private BarkManager barkManager;
    private bool soundHeard = false; //Si ha escuchado un sonido recientemente
    private Vector3 soundDirection; //Direcci�n de disho sonido
    private Vector3 previousEchoOrigin = Vector3.zero; //Transform que guarda la posici�n del �ltimo sonido escuchado, para que el lobo ignore sonidos consecutivos

    [Header("Behaviour: ")]
    [SerializeField] private bool isTactic = false; //Si es campero o no
    [SerializeField] private bool isIntelligent = false; //Si escucha los sonidos a trav�s de paredes o no
    public Vector3 initialPosition;
    public Quaternion initialRotation;

    
    void Awake()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        SetState(new StandingState(this));
        player = GameObject.FindWithTag("Player");
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        GameObject.FindObjectOfType<SideChanger>().sideChanged += ResetPosition;
    }

    void Update()
    {
        currentState.Update(Time.deltaTime);
    }

    private void FixedUpdate()
    {
        currentState.FixedUpdate(Time.fixedDeltaTime);
    }

    public void SetState(AWolfState state)
    {
        if (currentState != null) currentState.Exit();
        currentState = state;
        currentState.Enter();
    }

    public AWolfState GetState()
    {
        return currentState;
    }

    public Animator GetAnimator()
    {
        return anim;
    }

    public NavMeshAgent GetAgent()
    {
        return agent;
    }

    public Transform GetTransform()
    {
        return this.transform;
    }

    public bool PlayerOnSight()
    {
        //Si el jugador no es detectable por no ser golpeado por el echo la funci�n acaba
        if (!player.GetComponent<PlayerMovement>().canBeSeen) return false; 
        //Se saca el �ngulo entre la posici�n del jugador y los ojos del lobo
        Vector3 directionToPlayer = (player.transform.position - eyesTransform.position).normalized;
        float angleToPlayer = Vector3.Angle(eyesTransform.forward, directionToPlayer);
        //Si el �ngulo es menor a sightAngle significa que el jugador est� en radio de ser visto por el lobo
        if(angleToPlayer < sightAngle)
        {
            return PlayerNotHidden(); //Indica si entre el lobo y el jugador no hay ning�n obst�culo
        }
        return false;
    }

    private bool PlayerNotHidden()
    {
        //Se comprueba si no hay ning�n objeto en medio y el lobo puede ver al jugador
        RaycastHit hit;
        Vector3 directionToPlayer = (player.transform.position - eyesTransform.position).normalized;
        if (Physics.Raycast(eyesTransform.position, directionToPlayer, out hit, Mathf.Infinity, LayerMask.GetMask("Player", "Default")))
        {
            if (hit.transform.gameObject == player)
            {
                return true;
            }
        }
        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Echo"))
        {
            if (isIntelligent && !PlayerNotHidden()) return;
            if (other.transform.position != previousEchoOrigin) //Consecutivas ondas provenientes de la misma fuente de sonido son ignoradas
            {
                previousEchoOrigin = other.transform.position;
                soundDirection = other.transform.position - this.transform.position;
                StartCoroutine(SoundHeardWindow());
            }
        }
    }

    IEnumerator SoundHeardWindow() //Activa una ventana en la que el lobo est� atento del sonido y se gira hacia �l
    {
        soundHeard = true;
        yield return new WaitForSeconds(0.5f);
        soundHeard = false;
    }

    public bool SoundHeard()
    {
        return soundHeard;
    }

    public Quaternion GetSoundDirection()
    {
        return Quaternion.LookRotation(soundDirection);
    }

    public void PlayMouthAudio(string state)
    {
        barkManager.PlayMouthAudio(state);
    }

    public bool IsTactic()
    {
        return isTactic;
    }

    public void ResetPosition(object s, EventArgs e)
    {
        if (!isTactic) return;
        GetComponent<NavMeshAgent>().enabled = false;
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        GetComponent<NavMeshAgent>().enabled = true;
    }

    public bool IsIntelligent()
    {
        return isIntelligent;
    }
}
