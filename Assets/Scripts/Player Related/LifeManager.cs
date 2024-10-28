using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.AI;

public class LifeManager : MonoBehaviour
{
    [SerializeField] private GameObject deathCanvas;
    [SerializeField] private AudioSource gaspSource;
    [SerializeField] private AudioSource heartbeatSource;
    [SerializeField] private float beatInterval;
    private Vector3 lastCheckpoint = Vector3.zero;
    private PlayerMovement player;
    private SideChanger sideChanger;
    private EchoGenerator echoGenerator;

    private void Awake()
    {
        player = GameObject.FindObjectOfType<PlayerMovement>();
        sideChanger = GameObject.FindObjectOfType<SideChanger>();
        echoGenerator = GetComponent<EchoGenerator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Checkpoint"))
        {
            lastCheckpoint = other.transform.position;
            Destroy(other.gameObject);
        }
    }

    public void PlayerDeath()
    {
        if (GameObject.FindWithTag("MainCamera").GetComponent<Animator>().GetBool("Dead")) return;
        GameObject.FindWithTag("MainCamera").GetComponent<Animator>().SetBool("Dead", true);
        deathCanvas.SetActive(true);
        GameObject.FindObjectOfType<MainMenuManager>().OnDeath();
        //El personaje vuelve a estar de pie
        if (player.IsCrouching()) player.GetComponent<Animator>().SetTrigger("Toggle");
        sideChanger.PerformChange();
        Invoke("TpPlayer", 0.3f);
    }

    private void TpPlayer()
    {
        //Se hace tp al personaje a su checkpoint
        GetComponent<CharacterController>().enabled = false;
        transform.position = new Vector3(lastCheckpoint.x, transform.position.y, lastCheckpoint.z);
        transform.forward = -Vector3.forward;
        GetComponent<CharacterController>().enabled = true;
    }

    public void PlayAudios()
    {
        gaspSource.Play();
        StartCoroutine(Heartbeat());
    }

    IEnumerator Heartbeat()
    {
        float initialVolume = heartbeatSource.volume;
        while(heartbeatSource.volume > 0)
        {
            heartbeatSource.Play();
            heartbeatSource.volume -= 0.1f;
            echoGenerator.GenerateEcho(heartbeatSource.volume);
            yield return new WaitForSeconds(beatInterval);
        }
        heartbeatSource.volume = initialVolume;
    }
}
